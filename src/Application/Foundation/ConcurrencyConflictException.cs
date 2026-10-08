namespace GameNet.Application.Foundation;

public sealed class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException()
        : base("The requested operation conflicted with a newer state.")
    {
    }
}
