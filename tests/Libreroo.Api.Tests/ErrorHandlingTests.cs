using System.Net;

namespace Libreroo.Api.Tests;

public class ErrorHandlingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ErrorHandlingTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReturnLoan_WhenAlreadyReturned_ReturnsConflict()
    {
        var response = await _client.PostAsync("/loans/999/return", null);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
