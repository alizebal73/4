using GameNet.Domain.Shared;

namespace GameNet.Domain.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Addition_UsesIntegerTomanArithmetic()
    {
        var result = Money.FromToman(125_000) + Money.FromToman(75_000);
        Assert.Equal(200_000, result.Toman);
    }

    [Fact]
    public void Subtraction_PreservesSignedLedgerValues()
    {
        var result = Money.FromToman(125_000) - Money.FromToman(150_000);
        Assert.Equal(-25_000, result.Toman);
    }

    [Fact]
    public void Overflow_IsRejected()
    {
        Assert.Throws<OverflowException>(() => Money.FromToman(long.MaxValue) + Money.FromToman(1));
    }
}
