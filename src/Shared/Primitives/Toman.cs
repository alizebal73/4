namespace GameNet.Shared.Primitives;

public readonly record struct Toman(long Value)
{
    public static Toman Zero => new(0);

    public bool IsZero => Value == 0;

    public static Toman FromToman(long value) => new(value);

    public static Toman operator +(Toman left, Toman right) => new(checked(left.Value + right.Value));

    public static Toman operator -(Toman left, Toman right) => new(checked(left.Value - right.Value));

    public static Toman operator *(Toman value, long multiplier)
        => new(checked(value.Value * multiplier));

    public override string ToString() => Value.ToString();
}
