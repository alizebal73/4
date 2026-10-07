using System.Net.Http.Json;
using GameNet.Contracts.Foundation;

namespace GameNet.Desktop;

public partial class MainWindow
{
    private static readonly Uri ServerBaseUri =
        new("http://127.0.0.1:5080/");

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = ServerBaseUri,
        Timeout = TimeSpan.FromSeconds(5)
    };

    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (_, _) => await RefreshAsync();
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        StatusText.Text = "در حال بررسی...";

        try
        {
            var health = await _httpClient.GetFromJsonAsync<HealthResponse>("api/health");

            StatusText.Text = health is null
                ? "پاسخ نامعتبر"
                : $"Server={health.Status} | PostgreSQL={health.Database} | {health.UtcTime:HH:mm:ss}";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            StatusText.Text = "Server در دسترس نیست";
        }
    }
}
