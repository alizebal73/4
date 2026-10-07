namespace GameNet.Domain;

public readonly record struct Money(long Toman)
{
    public static Money Zero => new(0);

    public Money Add(Money other) => new(checked(Toman + other.Toman));

    public Money Subtract(Money other) => new(checked(Toman - other.Toman));

    public override string ToString() => Toman.ToString();
}
