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
public async Task GetTodos_Should_Return_All_Todos()
{
    // Arrange
    await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Gurenn Lagann"));
    await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Black Clover"));
    await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Inuyasha"));

    // Act
    var response = await _client.GetAsync("/api/todo");

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var todos =
        await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

    todos.Should().NotBeNull();
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
public async Task DeleteCompletedTodo_Should_Handle_Request()
{
    var createRequest1 = new CreateTodoRequest("Demon Slayer");
    var createRequest2 = new CreateTodoRequest("My Hero Academia");
    var createRequest3 = new CreateTodoRequest("Tokyo Revengers");

    await _client.PostAsJsonAsync("/api/todo", createRequest1);
    await _client.PostAsJsonAsync("/api/todo", createRequest2);
    await _client.PostAsJsonAsync("/api/todo", createRequest3);

    var updateAllStatusRequest = new UpdateAllStatusRequest(true);
    await _client.PatchAsJsonAsync("/api/todo/update-all", updateAllStatusRequest);

    var response =
        await _client.DeleteAsync("/api/todo/delete-completed");

    response.StatusCode.Should().BeOneOf(
        HttpStatusCode.NoContent,
        HttpStatusCode.BadRequest);

    if (response.StatusCode == HttpStatusCode.NoContent)
    {
        var todosResponse =
            await _client.GetAsync("/api/todo");

        var todos =
            await todosResponse.Content.ReadFromJsonAsync<List<TodoResponse>>();

        todos.Should().NotBeNull();
        todos!.Should().OnlyContain(t => !t.IsCompleted);
    }
}


 [Fact]
public async Task UpdateAllStatus_Should_Update_All_Todos_Status()
{
    // Arrange
    var createRequest1 = new CreateTodoRequest("Tokyo Ghoul");
    var createRequest2 = new CreateTodoRequest("Demon Slayer");
    var createRequest3 = new CreateTodoRequest("One Piece");

    var createResponse1 =
        await _client.PostAsJsonAsync("/api/todo", createRequest1);

    var createResponse2 =
        await _client.PostAsJsonAsync("/api/todo", createRequest2);

    var createResponse3 =
        await _client.PostAsJsonAsync("/api/todo", createRequest3);

    var todo1 =
        await createResponse1.Content.ReadFromJsonAsync<TodoResponse>();

    var todo2 =
        await createResponse2.Content.ReadFromJsonAsync<TodoResponse>();

    var todo3 =
        await createResponse3.Content.ReadFromJsonAsync<TodoResponse>();

    // Act - UNE seule requête pour tout mettre à completed
    var updateRequest = new UpdateAllStatusRequest(true);

    var updateResponse =
        await _client.PatchAsJsonAsync("/api/todo/update-all", updateRequest);

    // Assert HTTP response
    updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    // Vérifie que tout est bien marqué comme complété
    var response =
        await _client.GetAsync("/api/todo?isCompleted=true");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var completedTodos =
        await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

    completedTodos.Should().NotBeNull();

   completedTodos!.Count.Should().BeGreaterThanOrEqualTo(3);

    completedTodos.Should().Contain(t =>
        t.Id == todo1!.Id && t.IsCompleted);

    completedTodos.Should().Contain(t =>
        t.Id == todo2!.Id && t.IsCompleted);

    completedTodos.Should().Contain(t =>
        t.Id == todo3!.Id && t.IsCompleted);

    completedTodos.Should().OnlyContain(t => t.IsCompleted);

    completedTodos.Should().BeInAscendingOrder(t => t.CreatedAt);
}
}