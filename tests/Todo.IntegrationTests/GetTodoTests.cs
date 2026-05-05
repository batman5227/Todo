using System.Net;
using System.Net.Http.Json;
using Todo.Application.DTOs;
using Xunit;

namespace Todo.IntegrationTests;

public class GetTodosTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetTodosTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTodos_Should_Return_All_Todos()
    {

        var response = await _client.GetAsync("/api/todo");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("application/json", 
            response.Content.Headers.ContentType?.MediaType);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

        Assert.NotNull(todos);
        Assert.NotEmpty(todos);

        Assert.All(todos!, t =>
        {
            Assert.False(string.IsNullOrWhiteSpace(t.Name));
        });

        Assert.Contains(todos!, t => t.IsCompleted);
        Assert.Contains(todos!, t => !t.IsCompleted);
    }
}