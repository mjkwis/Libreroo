using System.Net;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Libreroo.Api.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests;

public class BooksEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BooksEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetBooks_ReturnsOk()
    {
        var subject = $"member-{Guid.NewGuid():N}";
        await SeedAccessUserAsync(subject, AccessRole.Member);
        var client = CreateAuthenticatedClient(subject);

        var response = await client.GetAsync("/books");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private HttpClient CreateAuthenticatedClient(string subject)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, subject);
        return client;
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
