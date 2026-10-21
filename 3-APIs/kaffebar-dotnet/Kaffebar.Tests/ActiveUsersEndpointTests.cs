using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Kaffebar.Tests;

public class ActiveUsersEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ActiveUsersEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveUsers_ReturnsOnlyActiveUsers()
    {
        var response = await _client.GetAsync("/users/active");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();

        Assert.NotNull(users);
        Assert.Collection(
            users,
            user =>
            {
                Assert.Equal(Guid.Parse("1e6d4562-9eaf-4ec3-8613-6f15afb92bd9"), user.Id);
                Assert.Equal("Ada Lovelace", user.Name);
                Assert.True(user.Active);
            },
            user =>
            {
                Assert.Equal(Guid.Parse("6b59c42b-4bf4-47b9-b28c-b2a3a95e91ef"), user.Id);
                Assert.Equal("Grace Hopper", user.Name);
                Assert.True(user.Active);
            });
    }

    private sealed record UserResponse(Guid Id, string Name, bool Active);
}
