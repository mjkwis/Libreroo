using Libreroo.Api.Modules.Access.Domain;

namespace Libreroo.Api.Modules.Access.Api;

public sealed record AccessUserResponse(
    Guid Id,
    string Subject,
    string? Email,
    string? DisplayName,
    int? MemberId,
    IReadOnlyList<string> Roles);

public sealed record AssignRoleRequest(Guid UserId, string Role);

public sealed record LinkMemberRequest(int MemberId);

internal static class AccessDtoMapper
{
    public static AccessUserResponse ToResponse(AccessUser user) =>
        new(
            user.Id,
            user.Subject,
            user.Email,
            user.DisplayName,
            user.MemberId,
            user.RoleAssignments
                .Select(assignment => assignment.Role.ToString().ToLowerInvariant())
                .Distinct()
                .OrderBy(role => role)
                .ToArray());
}
