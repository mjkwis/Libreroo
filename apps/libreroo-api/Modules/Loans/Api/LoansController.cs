using Libreroo.Api.Modules.Access.Application;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Modules.Loans.Application;
using Libreroo.Api.Shared.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Loans.Api;

[ApiController]
[Route("loans")]
[Authorize(Policy = AccessPolicies.MemberOrAbove)]
public sealed class LoansController : ControllerBase
{
    private readonly LoanService _loanService;
    private readonly CurrentUserContext _currentUserContext;

    public LoansController(LoanService loanService, CurrentUserContext currentUserContext)
    {
        _loanService = loanService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookCommand command, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetRequiredCurrentUserAsync(cancellationToken);
        var isElevatedUser = currentUser.HasRole(AccessRole.Librarian) || currentUser.HasRole(AccessRole.Admin);

        if (!isElevatedUser)
        {
            if (currentUser.MemberId is null)
            {
                return Forbid();
            }

            if (command.MemberId != currentUser.MemberId.Value)
            {
                return Forbid();
            }
        }

        var effectiveCommand = !isElevatedUser && currentUser.MemberId.HasValue
            ? command with { MemberId = currentUser.MemberId.Value }
            : command;

        var loanId = await _loanService.BorrowAsync(effectiveCommand, cancellationToken);
        return Created($"/loans/{loanId}", new { id = loanId });
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id, CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetRequiredCurrentUserAsync(cancellationToken);
        var isElevatedUser = currentUser.HasRole(AccessRole.Librarian) || currentUser.HasRole(AccessRole.Admin);

        if (!isElevatedUser)
        {
            if (currentUser.MemberId is null)
            {
                return Forbid();
            }

            var isOwner = await _loanService.IsLoanOwnedByMemberAsync(id, currentUser.MemberId.Value, cancellationToken);
            if (!isOwner)
            {
                return Forbid();
            }
        }

        await _loanService.ReturnAsync(new ReturnLoanCommand(id, DateTime.UtcNow), cancellationToken);
        return Ok();
    }

    [HttpGet("active")]
    public async Task<IActionResult> Active(CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetRequiredCurrentUserAsync(cancellationToken);
        var isElevatedUser = currentUser.HasRole(AccessRole.Librarian) || currentUser.HasRole(AccessRole.Admin);

        if (!isElevatedUser)
        {
            if (currentUser.MemberId is null)
            {
                return Forbid();
            }

            var ownLoans = await _loanService.GetActiveForMemberAsync(currentUser.MemberId.Value, cancellationToken);
            return Ok(ownLoans);
        }

        var loans = await _loanService.GetActiveAsync(cancellationToken);
        return Ok(loans);
    }

    [HttpGet("me/active")]
    public async Task<IActionResult> MyActive(CancellationToken cancellationToken)
    {
        var currentUser = await _currentUserContext.GetRequiredCurrentUserAsync(cancellationToken);
        if (currentUser.MemberId is null)
        {
            return Forbid();
        }

        var loans = await _loanService.GetActiveForMemberAsync(currentUser.MemberId.Value, cancellationToken);
        return Ok(loans);
    }
}
