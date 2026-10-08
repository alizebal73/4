namespace GameNet.Domain.Shared;

/// <summary>Canonical monetary value in integer Toman. No floating point.</summary>
public readonly record struct Money(long Toman)
{
    public static Money Zero => new(0);
    public static Money FromToman(long toman) => new(toman);
    public Money Abs() => new(Math.Abs(Toman));
    public static Money operator +(Money left, Money right) => new(checked(left.Toman + right.Toman));
    public static Money operator -(Money left, Money right) => new(checked(left.Toman - right.Toman));
    public static Money operator -(Money value) => new(checked(-value.Toman));
    public override string ToString() => $"{Toman:N0} تومان";
}
