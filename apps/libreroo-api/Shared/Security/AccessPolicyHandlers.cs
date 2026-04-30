using Libreroo.Api.Modules.Access.Application;
using Libreroo.Api.Shared.Application.Errors;
using Microsoft.AspNetCore.Authorization;

namespace Libreroo.Api.Shared.Security;

public sealed class MinimumRoleRequirementHandler : AuthorizationHandler<MinimumRoleRequirement>
{
    private readonly IAccessService _accessService;

    public MinimumRoleRequirementHandler(IAccessService accessService)
    {
        _accessService = accessService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumRoleRequirement requirement)
    {
        var subject = context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(subject))
        {
            return;
        }

        var cancellationToken = (context.Resource as HttpContext)?.RequestAborted ?? CancellationToken.None;

        try
        {
            var user = await _accessService.GetCurrentUserAsync(subject, cancellationToken);

            if (user.HasRole(requirement.MinimumRole))
            {
                context.Succeed(requirement);
            }
        }
        catch (DomainRuleViolationException)
        {
            // Authorization should remain forbidden when user is not provisioned.
        }
    }
}
