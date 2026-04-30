using Libreroo.Api.Shared.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Libreroo.Api.Tests;

public class DatabaseConnectivityTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DatabaseConnectivityTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DbContext_CanConnect()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibrerooDbContext>();
        var canConnect = await db.Database.CanConnectAsync();
        Assert.True(canConnect);
    }
}
