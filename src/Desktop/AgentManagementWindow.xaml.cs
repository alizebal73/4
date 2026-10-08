using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using GameNet.Contracts.Agents;
using GameNet.Contracts.Stations;
using GameNet.Desktop.Api;

namespace GameNet.Desktop;

public partial class AgentManagementWindow
{
    private readonly GameNetApiClient _apiClient;
    private readonly IReadOnlyList<StationDto> _stations;
    private IReadOnlyList<AgentDto> _agents = [];

    public AgentManagementWindow(
        GameNetApiClient apiClient,
        IReadOnlyList<StationDto> stations)
    {
        InitializeComponent();
        _apiClient = apiClient;
        _stations = stations;

        StationBox.ItemsSource = _stations;
        StationBox.DisplayMemberPath = nameof(StationDto.Name);

        Loaded += async (_, _) => await RefreshAsync();
    }

    private async void CreatePairingCode_Click(object sender, RoutedEventArgs e)
    {
        var deviceId = DeviceIdBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(deviceId))
        {
            StatusText.Text = "Device ID را وارد کنید.";
            return;
        }

        try
        {
            var pairing = await _apiClient.CreatePairingCodeAsync(
                deviceId,
                CancellationToken.None);

            PairingCodeBox.Text = pairing.PairingCode;
            StatusText.Text =
                $"کد تا {pairing.ExpiresAt.ToLocalTime():HH:mm:ss} معتبر است.";
            await RefreshAsync();
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException)
        {
            StatusText.Text = "ارتباط با Server برقرار نشد.";
        }
        catch (Exception exception)
        {
            StatusText.Text = exception.Message;
        }
    }

    private void AgentsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AgentsList.SelectedItem is AgentDto agent)
        {
            DeviceIdBox.Text = agent.DeviceId;
            SelectedAgentText.Text =
                $"{agent.DisplayName}
DeviceId: {agent.DeviceId}
وضعیت: {agent.State}";
            SelectStation(agent.StationId);
            BindButton.IsEnabled = agent.State != "Disabled";
        }
    }

    private async void BindButton_Click(object sender, RoutedEventArgs e)
    {
        if (AgentsList.SelectedItem is not AgentDto agent ||
            StationBox.SelectedItem is not StationDto station)
        {
            StatusText.Text = "Agent و ایستگاه مقصد را انتخاب کنید.";
            return;
        }

        try
        {
            BindButton.IsEnabled = false;
            await _apiClient.BindAgentAsync(
                agent.Id,
                station.Id,
                CancellationToken.None);

            StatusText.Text = "Agent با موفقیت به ایستگاه متصل شد.";
            await RefreshAsync();
        }
        catch (Exception exception)
        {
            StatusText.Text = exception.Message;
        }
        finally
        {
            BindButton.IsEnabled = true;
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e) =>
        await RefreshAsync();

    private async Task RefreshAsync()
    {
        try
        {
            var selectedId = (AgentsList.SelectedItem as AgentDto)?.Id;
            _agents = await _apiClient.GetAgentsAsync(CancellationToken.None);
            AgentsList.ItemsSource = _agents;

            if (selectedId is Guid id)
            {
                AgentsList.SelectedItem = _agents.FirstOrDefault(x => x.Id == id);
            }
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException)
        {
            StatusText.Text = "ارتباط با Server برقرار نشد.";
        }
    }

    private void SelectStation(Guid? stationId)
    {
        StationBox.SelectedItem = stationId is Guid id
            ? _stations.FirstOrDefault(x => x.Id == id)
            : null;
    }
}
