using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GameNet.Contracts.Stations;
using GameNet.Desktop.Api;

namespace GameNet.Desktop;

public partial class MainWindow
{
    private readonly GameNetApiClient _apiClient = new();
    private readonly DispatcherTimer _refreshTimer;
    private readonly List<StationDto> _allStations = [];

    private Guid? _editingStationId;
    private bool _creatingStation;

    public ObservableCollection<StationDto> Stations { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _refreshTimer.Tick += RefreshTimer_Tick;
    }

    protected override void OnClosed(EventArgs e)
    {
        _refreshTimer.Stop();
        base.OnClosed(e);
    }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        await ExecuteAsync(LoginButton, LoginStatus, async () =>
        {
            var response = await _apiClient.LoginAsync(
                UserNameBox.Text,
                PasswordBox.Password,
                CancellationToken.None);

            if (response is null)
            {
                LoginStatus.Text = "نام کاربری یا رمز عبور اشتباه است.";
                return;
            }

            OperatorText.Text =
                $"GameNet Manager 4 — {response.Operator.DisplayName} ({response.Operator.Role})";
            ConnectionText.Text = "Server متصل";
            LoginPanel.Visibility = Visibility.Collapsed;
            DashboardPanel.Visibility = Visibility.Visible;

            _refreshTimer.Start();
            await LoadStationsAsync();
        });
    }

    private async void RefreshStations_Click(object sender, RoutedEventArgs e)
    {
        await LoadStationsAsync();
    }

    private async void RefreshTimer_Tick(object? sender, EventArgs e)
    {
        if (_apiClient.IsAuthenticated)
        {
            await LoadStationsAsync();
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) =>
        ApplyFilter();

    private void StationCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Border { DataContext: StationDto station })
        {
            return;
        }

        OpenEditStation(station);
    }

    private void ManageAgents_Click(object sender, RoutedEventArgs e)
    {
        var window = new AgentManagementWindow(_apiClient, _allStations)
        {
            Owner = this
        };

        window.ShowDialog();
        _ = LoadStationsAsync();
    }

    private void NewStation_Click(object sender, RoutedEventArgs e)
    {
        _creatingStation = true;
        _editingStationId = null;
        EditorPanel.Visibility = Visibility.Visible;

        StationNumberBox.Text = string.Empty;
        StationNameBox.Text = string.Empty;
        SelectComboTag(StationTypeBox, "Pc");
        SelectComboTag(StationLifecycleBox, "Enabled");

        StationNumberBox.IsEnabled = true;
        DeleteStationButton.IsEnabled = false;
        SaveStationButton.Content = "ایجاد";
        SelectedStationText.Text = "ایستگاه جدید";
        StationNumberBox.Focus();
    }

    private void CancelEditor_Click(object sender, RoutedEventArgs e) =>
        CloseEditor();

    private async void SaveStation_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(StationNumberBox.Text.Trim(), out var number) || number <= 0)
        {
            SelectedStationText.Text = "شماره ایستگاه باید یک عدد مثبت باشد.";
            StationNumberBox.Focus();
            return;
        }

        var name = StationNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            SelectedStationText.Text = "نام ایستگاه الزامی است.";
            StationNameBox.Focus();
            return;
        }

        var type = ReadComboTag(StationTypeBox) ?? "Pc";
        var lifecycle = ReadComboTag(StationLifecycleBox) ?? "Enabled";

        await ExecuteAsync(SaveStationButton, SelectedStationText, async () =>
        {
            if (_creatingStation)
            {
                await _apiClient.CreateStationAsync(
                    new CreateStationRequest(number, name, type),
                    CancellationToken.None);
            }
            else if (_editingStationId is Guid stationId)
            {
                await _apiClient.UpdateStationAsync(
                    stationId,
                    new UpdateStationRequest(name, type, lifecycle),
                    CancellationToken.None);
            }

            CloseEditor();
            await LoadStationsAsync();
        });
    }

    private async void DeleteStation_Click(object sender, RoutedEventArgs e)
    {
        if (_editingStationId is not Guid stationId)
        {
            return;
        }

        var answer = MessageBox.Show(
            "این ایستگاه غیرفعال می‌شود. داده‌های تاریخی حذف نخواهند شد.",
            "غیرفعال‌سازی ایستگاه",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        await ExecuteAsync(DeleteStationButton, SelectedStationText, async () =>
        {
            await _apiClient.DeleteStationAsync(
                stationId,
                CancellationToken.None);

            CloseEditor();
            await LoadStationsAsync();
        });
    }

    private void OpenEditStation(StationDto station)
    {
        _creatingStation = false;
        _editingStationId = station.Id;
        EditorPanel.Visibility = Visibility.Visible;

        StationNumberBox.Text = station.Number.ToString();
        StationNumberBox.IsEnabled = false;
        StationNameBox.Text = station.Name;
        SelectComboTag(StationTypeBox, station.Type);
        SelectComboTag(StationLifecycleBox, station.Lifecycle);

        DeleteStationButton.IsEnabled =
            !string.Equals(station.Lifecycle, "Disabled", StringComparison.OrdinalIgnoreCase);
        SaveStationButton.Content = "ذخیره";

        SelectedStationText.Text =
            $"{station.Name} — Agent: {LocalizeAgentState(station.AgentState)}";
    }

    private void CloseEditor()
    {
        _creatingStation = false;
        _editingStationId = null;
        EditorPanel.Visibility = Visibility.Collapsed;
        SelectedStationText.Text = string.Empty;
        DeleteStationButton.IsEnabled = false;
    }

    private async Task LoadStationsAsync()
    {
        if (!_apiClient.IsAuthenticated)
        {
            return;
        }

        try
        {
            var selectedId = _editingStationId;

            var result = await _apiClient.GetStationsAsync(CancellationToken.None);

            _allStations.Clear();
            _allStations.AddRange(result);

            ApplyFilter();
            UpdateMetrics();

            if (selectedId is Guid id)
            {
                var selected = _allStations.FirstOrDefault(x => x.Id == id);
                if (selected is not null)
                {
                    SelectedStationText.Text =
                        $"{selected.Name} — Agent: {LocalizeAgentState(selected.AgentState)}";
                }
            }

            ConnectionText.Text = $"Server متصل • آخرین بازخوانی {DateTime.Now:HH:mm:ss}";
        }
        catch (UnauthorizedAccessException)
        {
            _refreshTimer.Stop();
            _allStations.Clear();
            Stations.Clear();
            DashboardPanel.Visibility = Visibility.Collapsed;
            LoginPanel.Visibility = Visibility.Visible;
            ConnectionText.Text = "نشست منقضی شده است";
            LoginStatus.Text = "نشست شما منقضی شده؛ دوباره وارد شوید.";
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException)
        {
            ConnectionText.Text = "Server در دسترس نیست";
            SelectedStationText.Text = "ارتباط با Server برقرار نشد؛ پس از برقراری ارتباط، بازخوانی خودکار ادامه دارد.";
        }
    }

    private void ApplyFilter()
    {
        var term = SearchBox.Text.Trim();

        Stations.Clear();

        foreach (var station in string.IsNullOrWhiteSpace(term)
                     ? _allStations
                     : _allStations.Where(x =>
                         x.Number.ToString().Contains(term, StringComparison.OrdinalIgnoreCase) ||
                         x.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                         LocalizeType(x.Type).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                         LocalizeLifecycle(x.Lifecycle).Contains(term, StringComparison.OrdinalIgnoreCase) ||
                         LocalizeAgentState(x.AgentState).Contains(term, StringComparison.OrdinalIgnoreCase)))
        {
            Stations.Add(station);
        }
    }

    private void UpdateMetrics()
    {
        TotalStationsText.Text = _allStations.Count.ToString();
        EnabledStationsText.Text = _allStations.Count(x =>
            string.Equals(x.Lifecycle, "Enabled", StringComparison.OrdinalIgnoreCase)).ToString();
        MaintenanceStationsText.Text = _allStations.Count(x =>
            string.Equals(x.Lifecycle, "Maintenance", StringComparison.OrdinalIgnoreCase)).ToString();
        OnlineAgentsText.Text = _allStations.Count(x =>
            string.Equals(x.AgentState, "Online", StringComparison.OrdinalIgnoreCase)).ToString();
    }

    private static string? ReadComboTag(ComboBox comboBox) =>
        comboBox.SelectedItem is ComboBoxItem { Tag: string tag } ? tag : null;

    private static void SelectComboTag(ComboBox comboBox, string value)
    {
        comboBox.SelectedItem = comboBox.Items
            .OfType<ComboBoxItem>()
            .FirstOrDefault(x => string.Equals(
                x.Tag as string,
                value,
                StringComparison.OrdinalIgnoreCase));
    }

    private static string LocalizeType(string value) =>
        value switch
        {
            "Pc" => "کامپیوتر",
            "Console" => "کنسول",
            "Foosball" => "فوتبال دستی",
            _ => value
        };

    private static string LocalizeLifecycle(string value) =>
        value switch
        {
            "Enabled" => "فعال",
            "Maintenance" => "تعمیرات",
            "Disabled" => "غیرفعال",
            _ => value
        };

    private static string LocalizeAgentState(string value) =>
        value switch
        {
            "Online" => "متصل",
            "Stale" => "قطع",
            "Paired" => "جفت‌شده",
            "Registered" => "ثبت‌شده",
            "Disabled" => "غیرفعال",
            "Unassigned" => "بدون Agent",
            _ => value
        };

    private static async Task ExecuteAsync(
        Button button,
        TextBlock status,
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
        catch (UnauthorizedAccessException)
        {
            status.Text = "نشست منقضی شده است.";
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
