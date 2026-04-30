using System.Net;
using System.Net.Http.Json;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests.Auth;

public class AccessAdminEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AccessAdminEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AssignRole_WithLibrarianRole_ReturnsForbidden()
    {
        var librarianSubject = $"librarian-{Guid.NewGuid():N}";
        var targetUserId = await SeedUserAsync($"target-{Guid.NewGuid():N}");
        await SeedUserAsync(librarianSubject, AccessRole.Librarian);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, librarianSubject);

        var response = await client.PostAsJsonAsync("/access/users/roles", new
        {
            userId = targetUserId,
            role = "admin"
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AssignRole_WithAdminRole_ReturnsOk()
    {
        var adminSubject = $"admin-{Guid.NewGuid():N}";
        var targetUserId = await SeedUserAsync($"target-{Guid.NewGuid():N}");
        await SeedUserAsync(adminSubject, AccessRole.Admin);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.SubjectHeader, adminSubject);

        var response = await client.PostAsJsonAsync("/access/users/roles", new
        {
            userId = targetUserId,
            role = "librarian"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<Guid> SeedUserAsync(string subject, AccessRole? role = null)
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();

        var accessUser = AccessUser.Create(subject, $"{subject}@demo.local", subject);
        if (role.HasValue)
        {
            accessUser.AssignRole(role.Value);
        }

        dbContext.AccessUsers.Add(accessUser);
        await dbContext.SaveChangesAsync();
        return accessUser.Id;
    }
}
