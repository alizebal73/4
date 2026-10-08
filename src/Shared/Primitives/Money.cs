namespace GameNet.Shared.Primitives;

public readonly record struct Money(long Toman)
{
    public static Money Zero => new(0);

    public static Money FromToman(long amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Money cannot be negative.");
        }

        return new Money(amount);
    }

    public static Money operator +(Money left, Money right) =>
        new(checked(left.Toman + right.Toman));

    public static Money operator -(Money left, Money right)
    {
        if (right.Toman > left.Toman)
        {
            throw new InvalidOperationException("Money subtraction cannot produce a negative amount.");
        }

        return new Money(left.Toman - right.Toman);
    }
}
