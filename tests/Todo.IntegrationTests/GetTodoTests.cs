using System.Net;
using System.Net.Http.Json;
using Todo.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace Todo.IntegrationTests;

public class GetTodosTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public GetTodosTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodo_Should_Return_CreatedTodo()
    {

        var createRequest = new CreateTodoRequest("Gurenn Lagann");

        var response = await _client.PostAsJsonAsync("/api/todo", createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        createdTodo.Should().NotBeNull();
        createdTodo!.Name.Should().Be(createRequest.Name);
        createdTodo.IsCompleted.Should().BeFalse();
        createdTodo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        createdTodo.UpdatedAt.Should().BeNull();
    }

    [Fact]
public async Task GetAllTodos_Should_Return_ListOfTodos()
{
    // Arrange
    var createRequest1 = new CreateTodoRequest("Soul eater");
    var createRequest2 = new CreateTodoRequest("Evangelion");
    var createRequest3 = new CreateTodoRequest("God Eater");
    var createRequest4 = new CreateTodoRequest("Grand blue");
    var createRequest5 = new CreateTodoRequest("Jujutsu Kaisen");
    var createRequest6 = new CreateTodoRequest("Naruto");

    await _client.PostAsJsonAsync("/api/todo", createRequest1);
    await _client.PostAsJsonAsync("/api/todo", createRequest2);
    await _client.PostAsJsonAsync("/api/todo", createRequest3);
    await _client.PostAsJsonAsync("/api/todo", createRequest4);
    await _client.PostAsJsonAsync("/api/todo", createRequest5);
    await _client.PostAsJsonAsync("/api/todo", createRequest6);

    // Act
    var response = await _client.GetAsync("/api/todo");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

    todos.Should().NotBeNull();
    todos.Should().HaveCountGreaterThanOrEqualTo(6);
    todos!.Should().Contain(t => t.Name == createRequest1.Name);
    todos.Should().Contain(t => t.Name == createRequest2.Name);
    todos.Should().Contain(t => t.Name == createRequest3.Name);
    todos.Should().Contain(t => t.Name == createRequest4.Name);
    todos.Should().Contain(t => t.Name == createRequest5.Name);
    todos.Should().Contain(t => t.Name == createRequest6.Name);
    todos.Should().BeInAscendingOrder(t => t.CreatedAt);
    todos.Should().OnlyHaveUniqueItems(t => t.Id);
    todos.Should().OnlyContain(t => t.CreatedAt <= DateTime.UtcNow);
    todos.Should().OnlyContain(t => t.UpdatedAt == null || t.UpdatedAt <= DateTime.UtcNow);
}
    [Fact]
    public async Task UpdateTodo_Should_Return_UpdatedTodo()
    {

        var createRequest = new CreateTodoRequest("Dragon Ball");
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var updateRequest = new UpdateTodoRequest("Castelvania");

        var response = await _client.PutAsJsonAsync($"/api/todo/{createdTodo!.Id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        updatedTodo.Should().NotBeNull();
        updatedTodo!.Name.Should().Be(updateRequest.Name);
        updatedTodo.IsCompleted.Should().BeFalse();
        updatedTodo.CreatedAt.Should().Be(createdTodo.CreatedAt);
    }

    [Fact]
    public async Task MarkAsCompleted_Should_Return_CompletedTodo()
    {

        var createRequest = new CreateTodoRequest("One Piece");
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var response = await _client.PatchAsync($"/api/todo/{createdTodo!.Id}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var completedTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        completedTodo.Should().NotBeNull();
        completedTodo!.IsCompleted.Should().BeTrue();
        completedTodo.CreatedAt.Should().Be(createdTodo.CreatedAt);
        completedTodo.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task MarkAsNotCompleted_Should_Return_NotCompletedTodo()
    {

        var createRequest = new CreateTodoRequest("Bleach");
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        await _client.PatchAsync($"/api/todo/{createdTodo!.Id}/complete", null);

        var response = await _client.PatchAsync($"/api/todo/{createdTodo.Id}/incomplete", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var notCompletedTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        notCompletedTodo.Should().NotBeNull();
        notCompletedTodo!.IsCompleted.Should().BeFalse();
        notCompletedTodo.CreatedAt.Should().Be(createdTodo.CreatedAt);
        notCompletedTodo.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
 [Fact]
   public async Task DeleteTodo_Should_Return_NoContent()
    {

        var createRequest = new CreateTodoRequest("Death Note");
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var response = await _client.DeleteAsync($"/api/todo/{createdTodo!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }   

    [Fact]
    public async Task DeleteCompletedTodo_Should_Return_NoContent()
    {

        var createRequest = new CreateTodoRequest("Samourai Champloo");
        var createResponse = await _client.PostAsJsonAsync("/api/todo", createRequest);
        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        await _client.PatchAsync($"/api/todo/{createdTodo!.Id}/complete", null);

        var response = await _client.DeleteAsync($"/api/todo/{createdTodo.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateAllStatus_Should_Return_UpdatedAllStatus()
    {

        var createRequest1 = new CreateTodoRequest("Tokyo Ghoul");
        var createRequest2 = new CreateTodoRequest("Demon Slayer");

        var createResponse1 = await _client.PostAsJsonAsync("/api/todo", createRequest1);
        var createdTodo1 = await createResponse1.Content.ReadFromJsonAsync<TodoResponse>();

        var createResponse2 = await _client.PostAsJsonAsync("/api/todo", createRequest2);
        var createdTodo2 = await createResponse2.Content.ReadFromJsonAsync<TodoResponse>();

        await _client.PatchAsync($"/api/todo/{createdTodo1!.Id}/complete", null);
        await _client.PatchAsync($"/api/todo/{createdTodo2!.Id}/complete", null);

        var response = await _client.GetAsync("/api/todo?isCompleted=true");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var completedTodos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        completedTodos.Should().NotBeNull();
        completedTodos!.Count.Should().BeGreaterThanOrEqualTo(2);
        completedTodos.Should().Contain(t => t.Id == createdTodo1.Id && t.IsCompleted);
        completedTodos.Should().Contain(t => t.Id == createdTodo2.Id && t.IsCompleted);
        completedTodos.Should().OnlyContain(t => t.IsCompleted);
        completedTodos.Should().BeInAscendingOrder(t => t.CreatedAt);
    }
    
}