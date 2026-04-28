using Libreroo.Api.Modules.Loans.Domain;
using Libreroo.Api.Shared.Application.Errors;

namespace Libreroo.Domain.Tests;

public class LoanTests
{
    [Fact]
    public void MarkReturned_SecondCall_Throws()
    {
        var loan = Loan.Create(bookId: 1, memberId: 2, borrowDateUtc: DateTime.UtcNow);
        loan.MarkReturned(DateTime.UtcNow.AddDays(1));

        Assert.Throws<InvalidOperationException>(() => loan.MarkReturned(DateTime.UtcNow.AddDays(2)));
    }

    [Fact]
    public void Borrow_WhenNoAvailableCopies_ThrowsDomainRuleViolation()
    {
        var book = new Libreroo.Api.Modules.Catalog.Domain.Book("DDD", authorId: 1, availableCopies: 0);
        Assert.Throws<DomainRuleViolationException>(() => book.DecreaseAvailableCopies());
    }
}
