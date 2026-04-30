using Libreroo.Api.Modules.Access.Domain;

namespace Libreroo.Api.Modules.Access.Application;

public interface IAccessService
{
    Task<IReadOnlyList<AccessUser>> GetUsersAsync(CancellationToken cancellationToken);

    Task<AccessUser> GetCurrentUserAsync(string subject, CancellationToken cancellationToken);

    Task<AccessUser> EnsureUserProfileAsync(
        string subject,
        string? email,
        string? displayName,
        CancellationToken cancellationToken);

    Task<AccessUser> AssignRoleAsync(Guid userId, AccessRole role, CancellationToken cancellationToken);

    Task<AccessUser> LinkMemberAsync(Guid userId, int memberId, CancellationToken cancellationToken);
}
