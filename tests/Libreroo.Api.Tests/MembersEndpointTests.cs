using System.Net;
using System.Net.Http.Json;

namespace Libreroo.Api.Tests;

public class MembersEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MembersEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostMembers_CreatesMember()
    {
        var response = await _client.PostAsJsonAsync("/members", new { fullName = "Ada Lovelace" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<MemberResponse>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal("Ada Lovelace", created.FullName);
    }

    private sealed record MemberResponse(int Id, string FullName);
}
