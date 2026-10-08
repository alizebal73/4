using GameNet.Domain.Identity;

namespace GameNet.Domain.Tests;

public sealed class OperatorTests
{
    [Fact]
    public void Operator_Permission_Is_Server_Derived()
    {
        var account = Operator.Create(
            "ADMIN",
            "GameNet Admin",
            "hash",
            OperatorRole.Manager,
            DateTimeOffset.UtcNow);

        Assert.True(account.HasPermission(Permission.AgentManage));
        Assert.True(account.HasPermission(Permission.StationWrite));
    }

    [Fact]
    public void Operator_Cannot_Use_Permission_After_Deactivation()
    {
        var account = Operator.Create(
            "OPERATOR",
            "Operator",
            "hash",
            OperatorRole.Operator,
            DateTimeOffset.UtcNow);

        account.Deactivate();

        Assert.False(account.HasPermission(Permission.StationRead));
    }
}
