using Xunit;

namespace GameNet.Domain.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Add_Uses_Integer_Toman_Semantics()
    {
        var a = new GameNet.Domain.Money(100_000);
        var b = new GameNet.Domain.Money(10_000);

        Assert.Equal(110_000, a.Add(b).Toman);
    }

    [Fact]
    public void Subtract_Preserves_Signed_Ledger_Amount()
    {
        var a = new GameNet.Domain.Money(100_000);
        var b = new GameNet.Domain.Money(125_000);

        Assert.Equal(-25_000, a.Subtract(b).Toman);
    }
}
