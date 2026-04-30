using Libreroo.Api.Modules.Access.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Libreroo.Api.Shared.Security;

public sealed class MinimumRoleRequirement : IAuthorizationRequirement
{
    public MinimumRoleRequirement(AccessRole minimumRole)
    {
        MinimumRole = minimumRole;
    }

    public AccessRole MinimumRole { get; }
}
