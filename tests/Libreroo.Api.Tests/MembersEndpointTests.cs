using System.Net;
using System.Net.Http.Json;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Libreroo.Api.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests;

public class MembersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MembersEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostMembers_CreatesMember()
    {
        var subject = $"librarian-{Guid.NewGuid():N}";
        await SeedAccessUserAsync(subject, AccessRole.Librarian);
        var client = CreateAuthenticatedClient(subject);

        var response = await client.PostAsJsonAsync("/members", new { fullName = "Ada Lovelace" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<MemberResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Ada Lovelace", created.FullName);
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

    private sealed record MemberResponse(int Id, string FullName);
}
