using System.Net;

namespace Libreroo.Api.Tests;

public class LoansEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoansEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveLoans_ReturnsOk()
    {
        var response = await _client.GetAsync("/loans/active");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
