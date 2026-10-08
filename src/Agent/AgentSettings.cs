namespace GameNet.Agent;

public sealed class AgentSettings
{
    public AgentSettings(string serverUrl, string accessToken)
    {
        ServerUrl = serverUrl.TrimEnd('/');
        AccessToken = accessToken;
    }

    public string ServerUrl { get; }
    public string AccessToken { get; }

    public static AgentSettings FromEnvironment(string? accessToken)
    {
        var serverUrl = Environment.GetEnvironmentVariable("GAMENET_AGENT_SERVER")
            ?? "http://127.0.0.1:5080";

        return new AgentSettings(serverUrl, accessToken ?? string.Empty);
    }
}
