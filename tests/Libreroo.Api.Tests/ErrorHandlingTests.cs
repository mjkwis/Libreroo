using System.Net;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Libreroo.Api.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests;

public class ErrorHandlingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ErrorHandlingTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ReturnLoan_WhenAlreadyReturned_ReturnsConflict()
    {
        var subject = $"librarian-{Guid.NewGuid():N}";
        await SeedAccessUserAsync(subject, AccessRole.Librarian);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, subject);

        var response = await client.PostAsync("/loans/999/return", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task SeedAccessUserAsync(string subject, AccessRole role)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();

        var accessUser = AccessUser.Create(subject, $"{subject}@demo.local", subject);
        accessUser.AssignRole(role);
        dbContext.AccessUsers.Add(accessUser);
        await dbContext.SaveChangesAsync();
    }
}
