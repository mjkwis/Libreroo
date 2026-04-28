using System.Net;

namespace Libreroo.Api.Tests;

public class BooksEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ReturnsOk()
    {
        var response = await _client.GetAsync("/books");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
