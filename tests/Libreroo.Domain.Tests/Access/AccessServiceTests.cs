using Libreroo.Api.Modules.Access.Application;
using Libreroo.Api.Modules.Access.Domain;
using Libreroo.Api.Shared.Application.Errors;
using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Libreroo.Domain.Tests.Access;

public class AccessServiceTests
{
    [Fact]
    public async Task GetCurrentUser_WhenMissingSubject_ThrowsForbiddenRule()
    {
        await using var dbContext = CreateDbContext();
        var service = new AccessService(dbContext);

        await Assert.ThrowsAsync<DomainRuleViolationException>(
            () => service.GetCurrentUserAsync("missing-sub", default));
    }

    [Fact]
    public async Task EnsureUserProfile_WhenMissing_CreatesUserWithoutRoles()
    {
        await using var dbContext = CreateDbContext();
        var service = new AccessService(dbContext);

        var user = await service.EnsureUserProfileAsync("sub-123", "member@demo.local", "Member User", default);

        Assert.Equal("sub-123", user.Subject);
        Assert.Equal("member@demo.local", user.Email);
        Assert.Equal("Member User", user.DisplayName);
        Assert.Empty(user.RoleAssignments);
    }

    [Fact]
    public async Task GetCurrentUser_WhenExisting_ReturnsUser()
    {
        await using var dbContext = CreateDbContext();
        var accessUser = AccessUser.Create("sub-123", "member@demo.local", "Member User");
        dbContext.AccessUsers.Add(accessUser);
        await dbContext.SaveChangesAsync();

        var service = new AccessService(dbContext);
        var user = await service.GetCurrentUserAsync("sub-123", default);

        Assert.Equal(accessUser.Subject, user.Subject);
    }

    private static LibrerooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LibrerooDbContext>()
            .UseInMemoryDatabase($"access-service-tests-{Guid.NewGuid():N}")
            .Options;

        return new LibrerooDbContext(options);
    }
}
