using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using GameNet.Contracts.Stations;
using GameNet.Desktop.Api;

namespace GameNet.Desktop;

public partial class MainWindow
{
    private readonly GameNetApiClient _apiClient = new();

    public ObservableCollection<StationDto> Stations { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        await ExecuteAsync(
            LoginButton,
            LoginStatus,
            async () => await LoginAsync(
                UserNameBox.Text,
                PasswordBox.Password));
    }

    private async void RefreshStations_Click(object sender, RoutedEventArgs e)
    {
        await LoadStationsAsync();
    }

    private async Task LoginAsync(string userName, string password)
    {
        var response = await _apiClient.LoginAsync(
            userName,
            password,
            CancellationToken.None);

        if (response is null)
        {
            LoginStatus.Text = "نام کاربری یا رمز عبور اشتباه است.";
            return;
        }

        OperatorText.Text =
            $"GameNet Manager 4 — {response.Operator.DisplayName} ({response.Operator.Role})";

        LoginPanel.Visibility = Visibility.Collapsed;
        DashboardPanel.Visibility = Visibility.Visible;

        await LoadStationsAsync();
    }

    private async Task LoadStationsAsync()
    {
        try
        {
            Stations.Clear();

            foreach (var station in await _apiClient.GetStationsAsync(CancellationToken.None))
            {
                Stations.Add(station);
            }

            LoginStatus.Text = $"تعداد ایستگاه‌ها: {Stations.Count}";
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException)
        {
            LoginStatus.Text = "Server در دسترس نیست.";
        }
    }

    private static async Task ExecuteAsync(
        System.Windows.Controls.Button button,
        System.Windows.Controls.TextBlock status,
        Func<Task> operation)
    {
        button.IsEnabled = false;
        status.Text = "در حال انجام...";

        try
        {
            await operation();
        }
        catch (HttpRequestException)
        {
            status.Text = "ارتباط با Server برقرار نشد.";
        }
        catch (TaskCanceledException)
        {
            status.Text = "درخواست منقضی شد.";
        }
        catch (Exception exception)
        {
            status.Text = exception.Message;
        }
        finally
        {
            button.IsEnabled = true;
        }
    }
}
