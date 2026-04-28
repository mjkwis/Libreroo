using Libreroo.Api.Modules.Loans.Application;
using Microsoft.AspNetCore.Mvc;

namespace Libreroo.Api.Modules.Loans.Api;

[ApiController]
[Route("loans")]
public sealed class LoansController : ControllerBase
{
    private readonly LoanService _loanService;

    public LoansController(LoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookCommand command, CancellationToken cancellationToken)
    {
        var loanId = await _loanService.BorrowAsync(command, cancellationToken);
        return Created($"/loans/{loanId}", new { id = loanId });
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id, CancellationToken cancellationToken)
    {
        await _loanService.ReturnAsync(new ReturnLoanCommand(id, DateTime.UtcNow), cancellationToken);
        return Ok();
    }

    [HttpGet("active")]
    public async Task<IActionResult> Active(CancellationToken cancellationToken)
    {
        var loans = await _loanService.GetActiveAsync(cancellationToken);
        return Ok(loans);
    }
}
