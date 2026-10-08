using Xunit;

using GameNet.Domain.Agents;

namespace GameNet.Domain.Tests;

public sealed class AgentDeviceTests
{
    [Fact]
    public void Old_Connection_Cannot_Heartbeat_After_New_Lease()
    {
        var now = DateTimeOffset.Parse("2026-10-08T00:00:00Z");
        var agent = AgentDevice.Register("PC-01", now);

        agent.CompletePairing("PC 01", "credential");
        var firstLease = agent.OpenLease("connection-a", now);

        var secondLease = agent.OpenLease("connection-b", now.AddSeconds(1));

        Assert.NotEqual(firstLease, secondLease);
        Assert.False(agent.Heartbeat("connection-a", firstLease, now.AddSeconds(2)));
        Assert.True(agent.Heartbeat("connection-b", secondLease, now.AddSeconds(2)));
    }

    [Fact]
    public void Expired_Pairing_Code_Cannot_Pair()
    {
        var created = DateTimeOffset.Parse("2026-10-08T00:00:00Z");
        var agent = AgentDevice.Register("PC-02", created);

        agent.SetPairingCode("hash", created.AddMinutes(1));

        Assert.False(agent.CanPair("hash", created.AddMinutes(1)));
        Assert.True(agent.CanPair("hash", created.AddSeconds(30)));
    }
}

    [Fact]
    public void Online_Agent_Becomes_Stale_After_Heartbeat_Timeout()
    {
        var created = DateTimeOffset.Parse("2026-10-08T00:00:00Z");
        var agent = AgentDevice.Register("PC-03", created);

        agent.CompletePairing("PC 03", "credential");
        agent.OpenLease("connection-a", created);

        Assert.True(agent.ShouldBecomeStale(created.AddSeconds(31), TimeSpan.FromSeconds(30)));

        agent.MarkStale(created.AddSeconds(31));

        Assert.Equal(AgentState.Stale, agent.State);
        Assert.Null(agent.ConnectionId);
        Assert.False(agent.Heartbeat("connection-a", 1, created.AddSeconds(32)));
    }
