using System.Net;
using System.Net.Http.Json;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Modules.Catalog.Domain;
using Libreroo.Api.Modules.Loans.Domain;
using Libreroo.Api.Modules.Members.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests.Auth;

public class LoanAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LoanAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Member_CannotBorrowOnBehalfOfAnotherMember()
    {
        var subject = $"member-{Guid.NewGuid():N}";
        var seed = await SeedBorrowScenarioAsync(subject);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, subject);

        var response = await client.PostAsJsonAsync("/loans", new
        {
            bookId = seed.BookId,
            memberId = seed.MemberId + 100,
            borrowDateUtc = DateTime.UtcNow
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetMyActive_ForMember_ReturnsOnlyOwnLoans()
    {
        var subject = $"member-{Guid.NewGuid():N}";
        var expectedMemberId = await SeedMyActiveScenarioAsync(subject);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, subject);

        var response = await client.GetAsync("/loans/me/active");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<List<LoanPayload>>();
        Assert.NotNull(payload);
        Assert.Single(payload!);
        Assert.Equal(expectedMemberId, payload[0].MemberId);
    }

    private async Task<(int MemberId, int BookId)> SeedBorrowScenarioAsync(string subject)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();

        var member = new Member("Member One");
        dbContext.Members.Add(member);

        var author = new Author("Author One");
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        var book = new Book("Book One", author.Id, 2);
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync();

        var accessUser = AccessUser.Create(subject, $"{subject}@demo.local", subject);
        accessUser.LinkMember(member.Id);
        accessUser.AssignRole(AccessRole.Member);

        dbContext.AccessUsers.Add(accessUser);
        await dbContext.SaveChangesAsync();

        return (member.Id, book.Id);
    }

    private async Task<int> SeedMyActiveScenarioAsync(string subject)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();

        var member1 = new Member("Member One");
        var member2 = new Member("Member Two");
        dbContext.Members.AddRange(member1, member2);

        var author = new Author("Author One");
        dbContext.Authors.Add(author);
        await dbContext.SaveChangesAsync();

        var book1 = new Book("Book One", author.Id, 2);
        var book2 = new Book("Book Two", author.Id, 2);
        dbContext.Books.AddRange(book1, book2);
        await dbContext.SaveChangesAsync();

        dbContext.Loans.Add(Loan.Create(book1.Id, member1.Id, DateTime.UtcNow.AddDays(-1)));
        dbContext.Loans.Add(Loan.Create(book2.Id, member2.Id, DateTime.UtcNow.AddDays(-1)));

        var accessUser = AccessUser.Create(subject, $"{subject}@demo.local", subject);
        accessUser.LinkMember(member1.Id);
        accessUser.AssignRole(AccessRole.Member);
        dbContext.AccessUsers.Add(accessUser);

        await dbContext.SaveChangesAsync();
        return member1.Id;
    }

    private sealed record LoanPayload(int Id, int BookId, int MemberId, DateTime BorrowDate, DateTime? ReturnDate);
}
