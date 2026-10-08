using System.Net.Http.Json;
using GameNet.Agent.Security;
using GameNet.Contracts.Agents;

namespace GameNet.Agent.Provisioning;

public sealed class AgentProvisioner(AgentCredentialStore credentialStore)
{
    public async Task<bool> PairAsync(
        string serverUrl,
        string deviceId,
        string pairingCode,
        string displayName,
        CancellationToken cancellationToken)
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri(serverUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(10)
        };

        var response = await client.PostAsJsonAsync(
            "api/agents/pair",
            new PairAgentRequest(deviceId, pairingCode, displayName),
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<PairAgentResponse>(
            cancellationToken: cancellationToken);

        if (result is null || string.IsNullOrWhiteSpace(result.AccessToken))
        {
            return false;
        }

        credentialStore.Save(result.AccessToken);
        return true;
    }
}
