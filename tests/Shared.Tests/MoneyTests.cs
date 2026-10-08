using GameNet.Shared.Primitives;

namespace GameNet.Shared.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Addition_is_integer_and_exact()
    {
        var total = Money.FromToman(1000) + Money.FromToman(2500);

        Assert.Equal(3500, total.Toman);
    }

    [Fact]
    public void Negative_amount_is_rejected_by_factory()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Money.FromToman(-1));
    }
}
