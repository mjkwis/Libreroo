using Libreroo.Api.Modules.Members.Application;
using Libreroo.Api.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Members.Api;

[ApiController]
[Route("members")]
[Authorize(Policy = AccessPolicies.LibrarianOrAdmin)]
public sealed class MembersController : ControllerBase
{
    private readonly MemberService _memberService;

    public MembersController(MemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var members = await _memberService.GetMembersAsync(cancellationToken);
        return Ok(members);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMemberRequest request, CancellationToken cancellationToken)
    {
        var member = await _memberService.CreateMemberAsync(request.FullName, cancellationToken);
        return Created($"/members/{member.Id}", member);
    }

    public sealed record CreateMemberRequest(string FullName);
}
