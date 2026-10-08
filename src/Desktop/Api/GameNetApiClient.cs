using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GameNet.Contracts.Agents;
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

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken);

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
        var response = await SendAuthenticatedAsync(
            HttpMethod.Get,
            "api/stations",
            cancellationToken);

        return await response.Content.ReadFromJsonAsync<List<StationDto>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    public async Task<StationDto> CreateStationAsync(
        CreateStationRequest request,
        CancellationToken cancellationToken)
    {
        using var message = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "api/stations");

        message.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StationDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Server returned an empty station.");
    }

    public async Task<StationDto> UpdateStationAsync(
        Guid stationId,
        UpdateStationRequest request,
        CancellationToken cancellationToken)
    {
        using var message = CreateAuthenticatedRequest(
            HttpMethod.Put,
            $"api/stations/{stationId}");

        message.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StationDto>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Server returned an empty station.");
    }

    public async Task DeleteStationAsync(
        Guid stationId,
        CancellationToken cancellationToken)
    {
        using var message = CreateAuthenticatedRequest(
            HttpMethod.Delete,
            $"api/stations/{stationId}");

        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<AgentDto>> GetAgentsAsync(
        CancellationToken cancellationToken)
    {
        var response = await SendAuthenticatedAsync(
            HttpMethod.Get,
            "api/agents",
            cancellationToken);

        return await response.Content.ReadFromJsonAsync<List<AgentDto>>(
                   cancellationToken: cancellationToken)
               ?? [];
    }

    public async Task<CreatePairingCodeResponse> CreatePairingCodeAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        using var message = CreateAuthenticatedRequest(
            HttpMethod.Post,
            "api/agents/pairing-code");

        message.Content = JsonContent.Create(new CreatePairingCodeRequest(deviceId));

        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CreatePairingCodeResponse>(
                   cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Server returned an empty pairing response.");
    }

    public async Task BindAgentAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken)
    {
        using var message = CreateAuthenticatedRequest(
            HttpMethod.Post,
            $"api/agents/{agentId}/bind");

        message.Content = JsonContent.Create(new BindAgentRequest(stationId));

        var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpResponseMessage> SendAuthenticatedAsync(
        HttpMethod method,
        string path,
        CancellationToken cancellationToken)
    {
        using var request = CreateAuthenticatedRequest(method, path);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Desktop authentication expired.");
        }

        response.EnsureSuccessStatusCode();
        return response;
    }

    private HttpRequestMessage CreateAuthenticatedRequest(
        HttpMethod method,
        string path)
    {
        EnsureAuthenticated();

        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);
        return request;
    }

    private void EnsureAuthenticated()
    {
        if (string.IsNullOrWhiteSpace(_accessToken))
        {
            throw new InvalidOperationException("Desktop is not authenticated.");
        }
    }
}
