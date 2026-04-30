namespace Libreroo.Api.Modules.Access.Domain;

public sealed class AccessRoleAssignment
{
    private AccessRoleAssignment()
    {
    }

    private AccessRoleAssignment(AccessRole role)
    {
        Role = role;
        AssignedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid AccessUserId { get; private set; }

    public AccessRole Role { get; private set; }

    public DateTime AssignedAtUtc { get; private set; }

    public AccessUser AccessUser { get; private set; } = null!;

    public static AccessRoleAssignment Create(AccessRole role) => new(role);
}
