namespace GameNet.Domain.Identity;

public sealed class Operator
{
    private Operator() { }

    private Operator(
        Guid id,
        string userName,
        string displayName,
        string passwordHash,
        OperatorRole role,
        DateTimeOffset createdAt)
    {
        Id = id;
        UserName = userName;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public OperatorRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Operator Create(
        string userName,
        string displayName,
        string passwordHash,
        OperatorRole role,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentException("Username is required.", nameof(userName));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Display name is required.", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        }

        return new Operator(
            Guid.NewGuid(),
            userName.Trim(),
            displayName.Trim(),
            passwordHash,
            role,
            createdAt);
    }

    public bool HasPermission(Permission permission) =>
        IsActive && RolePermissions.Has(Role, permission);

    public void Deactivate() => IsActive = false;
}
