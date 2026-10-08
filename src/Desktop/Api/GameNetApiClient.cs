using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameNet.Contracts.Identity;
using GameNet.Contracts.Stations;

namespace GameNet.Desktop.Api;

public sealed class GameNetApiClient
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri(
            Environment.GetEnvironmentVariable("GAMENET_SERVER_URL")
            ?? "http://127.0.0.1:5080/"),
        Timeout = TimeSpan.FromSeconds(5)
    };

    private string? _accessToken;

    public async Task<LoginResponse?> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/login",
            new LoginRequest(userName, password),
            cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>(
            cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException("Server returned an empty login response.");
        }

        _accessToken = result.AccessToken;
        return result;
    }

    public async Task<IReadOnlyList<StationDto>> GetStationsAsync(
        CancellationToken cancellationToken)
    {
        EnsureAuthenticated();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/stations");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<StationDto>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new InvalidOperationException("Desktop is not authenticated.");
        }
    }
}
