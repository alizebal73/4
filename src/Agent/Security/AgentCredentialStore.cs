using System.Security.Cryptography;
using System.Text;

namespace GameNet.Agent.Security;

public sealed class AgentCredentialStore
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("GameNet.Agent.v1");
    private readonly string _path;

    public AgentCredentialStore()
    {
        var root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GameNet Manager",
            "Agent");

        Directory.CreateDirectory(root);
        _path = Path.Combine(root, "credential.bin");
    }

    public void Save(string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        var plaintext = Encoding.UTF8.GetBytes(accessToken);
        var protectedBytes = ProtectedData.Protect(
            plaintext,
            Entropy,
            DataProtectionScope.LocalMachine);

        File.WriteAllBytes(_path, protectedBytes);
        CryptographicOperations.ZeroMemory(plaintext);
    }

    public string? TryRead()
    {
        if (!File.Exists(_path))
        {
            return null;
        }

        try
        {
            var protectedBytes = File.ReadAllBytes(_path);
            var plaintext = ProtectedData.Unprotect(
                protectedBytes,
                Entropy,
                DataProtectionScope.LocalMachine);

            try
            {
                return Encoding.UTF8.GetString(plaintext);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }
        }
        catch (CryptographicException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
    }
}
