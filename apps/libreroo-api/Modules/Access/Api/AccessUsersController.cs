using Libreroo.Api.Modules.Access.Application;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Access.Api;

[ApiController]
[Route("access/users")]
[Authorize(Policy = AccessPolicies.AdminOnly)]
public sealed class AccessUsersController : ControllerBase
{
    private readonly IAccessService _accessService;

    public AccessUsersController(IAccessService accessService)
    {
        _accessService = accessService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var users = await _accessService.GetUsersAsync(cancellationToken);
        return Ok(users.Select(AccessDtoMapper.ToResponse));
    }

    [HttpPost("roles")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AccessRole>(request.Role, true, out var role))
        {
            return BadRequest(new { error = $"Unsupported role '{request.Role}'." });
        }

        var user = await _accessService.AssignRoleAsync(request.UserId, role, cancellationToken);
        return Ok(AccessDtoMapper.ToResponse(user));
    }

    [HttpPost("{userId:guid}/member-link")]
    public async Task<IActionResult> LinkMember(Guid userId, [FromBody] LinkMemberRequest request, CancellationToken cancellationToken)
    {
        var user = await _accessService.LinkMemberAsync(userId, request.MemberId, cancellationToken);
        return Ok(AccessDtoMapper.ToResponse(user));
    }
}
