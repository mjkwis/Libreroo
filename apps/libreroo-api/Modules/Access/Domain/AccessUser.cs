namespace Libreroo.Api.Modules.Access.Domain;

public sealed class AccessUser
{
    private readonly List<AccessRoleAssignment> _roleAssignments = [];

    private AccessUser()
    {
    }

    private AccessUser(string subject, string? email, string? displayName)
    {
        Subject = subject;
        Email = email;
        DisplayName = displayName;
    }

    public Guid Id { get; private set; }

    public string Subject { get; private set; } = string.Empty;

    public string? Email { get; private set; }

    public string? DisplayName { get; private set; }

    public int? MemberId { get; private set; }

    public IReadOnlyCollection<AccessRoleAssignment> RoleAssignments => _roleAssignments;

    public static AccessUser Create(string subject, string? email, string? displayName)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Subject is required.", nameof(subject));
        }

        return new AccessUser(subject.Trim(), email?.Trim(), displayName?.Trim());
    }

    public void UpdateProfile(string? email, string? displayName)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    public void LinkMember(int memberId)
    {
        if (memberId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(memberId), "MemberId must be greater than zero.");
        }

        MemberId = memberId;
    }

    public void AssignRole(AccessRole role)
    {
        if (_roleAssignments.Any(assignment => assignment.Role == role))
        {
            return;
        }

        _roleAssignments.Add(AccessRoleAssignment.Create(role));
    }

    public bool HasRole(AccessRole requiredRole)
    {
        if (_roleAssignments.Count == 0)
        {
            return false;
        }

        return _roleAssignments.Any(assignment => assignment.Role >= requiredRole);
    }
}
