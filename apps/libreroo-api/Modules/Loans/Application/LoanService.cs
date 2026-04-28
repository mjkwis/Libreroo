using Libreroo.Api.Modules.Loans.Domain;
using Libreroo.Api.Shared.Application.Errors;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Api.Modules.Loans.Application;

public sealed class LoanService
{
    private readonly LibrerooDbContext _dbContext;

    public LoanService(LibrerooDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> BorrowAsync(BorrowBookCommand command, CancellationToken cancellationToken = default)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == command.BookId, cancellationToken);
        if (book is null)
        {
            throw new DomainRuleViolationException("Book not found.");
        }

        var memberExists = await _dbContext.Members.AnyAsync(member => member.Id == command.MemberId, cancellationToken);
        if (!memberExists)
        {
            throw new DomainRuleViolationException("Member not found.");
        }

        book.DecreaseAvailableCopies();

        var loan = Loan.Create(command.BookId, command.MemberId, command.BorrowDateUtc);
        _dbContext.Loans.Add(loan);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return loan.Id;
    }

    public async Task ReturnAsync(ReturnLoanCommand command, CancellationToken cancellationToken = default)
    {
        var loan = await _dbContext.Loans.FirstOrDefaultAsync(item => item.Id == command.LoanId, cancellationToken);
        if (loan is null)
        {
            throw new DomainRuleViolationException("Loan not found.");
        }

        try
        {
            loan.MarkReturned(command.ReturnDateUtc);
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainRuleViolationException(ex.Message);
        }

        var book = await _dbContext.Books.FirstAsync(item => item.Id == loan.BookId, cancellationToken);
        book.IncreaseAvailableCopies();

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Loan>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        _dbContext.Loans
            .AsNoTracking()
            .Where(loan => loan.ReturnDate == null)
            .ToListAsync(cancellationToken);
}
