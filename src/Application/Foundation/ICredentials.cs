namespace GameNet.Application.Foundation;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface ITokenGenerator
{
    string Generate();
    string Hash(string token);
}
