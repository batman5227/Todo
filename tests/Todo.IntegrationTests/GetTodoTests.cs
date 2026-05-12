using System.Net;
using System.Net.Http.Json;
using Todo.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace Todo.IntegrationTests;

public class GetTodosTests : IClassFixture<CustomWebApplicationFactory>
{
    private sealed record ErrorResponse(string? error);

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
    public async Task GetTodos_Should_Return_All_Todos_When_IsCompleted_Is_Null()
    {
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Gurenn Lagann"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Black Clover"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Inuyasha"));

        var response = await _client.GetAsync("/api/todo");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();

        todos.Should().Contain(t => t.Name == "Gurenn Lagann");
        todos.Should().Contain(t => t.Name == "Black Clover");
        todos.Should().Contain(t => t.Name == "Inuyasha");
    }

    [Fact]
    public async Task GetTodos_Should_Return_All_Todos()
    {
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Gurenn Lagann"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Black Clover"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Inuyasha"));

        var response = await _client.GetAsync("/api/todo");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();

        todos.Should().Contain(t => t.Name == "Gurenn Lagann");
        todos.Should().Contain(t => t.Name == "Black Clover");
        todos.Should().Contain(t => t.Name == "Inuyasha");

        todos.Should().BeOfType<List<TodoResponse>>();
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
    public async Task DeleteAllCompleted_Should_Remove_All_Completed_Todos_And_Keep_NotCompleted()
    {
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Black Star"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Shinra Kusakabe"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Arthur Boyle"));

        await _client.PatchAsJsonAsync("/api/todo/update-all", new UpdateAllStatusRequest(true));

        var beforeDeleteResponse = await _client.GetAsync("/api/todo?filter=completed");
        var completedBefore = await beforeDeleteResponse.Content.ReadFromJsonAsync<List<TodoResponse>>();
        completedBefore.Should().NotBeNull();
        completedBefore.Should().NotBeEmpty();

        var deleteResponse = await _client.DeleteAsync("/api/todo/completed");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var afterCompleted = await (await _client.GetAsync("/api/todo?filter=completed"))
            .Content.ReadFromJsonAsync<List<TodoResponse>>();
        afterCompleted.Should().NotBeNull();
        afterCompleted!.Should().BeEmpty();

        var allTodos = await (await _client.GetAsync("/api/todo"))
            .Content.ReadFromJsonAsync<List<TodoResponse>>();
        allTodos.Should().NotBeNull();
        allTodos!.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAllStatus_Should_Update_All_Todos_To_Completed_Or_NotCompleted()
    {
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Tokyo Ghoul"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Demon Slayer"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("One Piece"));

        var updateResponse = await _client.PatchAsJsonAsync("/api/todo/update-all", new UpdateAllStatusRequest(true));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await (await _client.GetAsync("/api/todo"))
            .Content.ReadFromJsonAsync<List<TodoResponse>>();

        todos.Should().NotBeNull();
        todos!.Should().OnlyContain(t => t.IsCompleted);

        var updateResponseFalse = await _client.PatchAsJsonAsync("/api/todo/update-all", new UpdateAllStatusRequest(false));
        updateResponseFalse.StatusCode.Should().Be(HttpStatusCode.OK);

        var notCompleted = await (await _client.GetAsync("/api/todo?filter=active"))
            .Content.ReadFromJsonAsync<List<TodoResponse>>();

        notCompleted.Should().NotBeNull();
        notCompleted!.Should().OnlyContain(t => !t.IsCompleted);
    }

    [Fact]
    public async Task GetTodos_When_Filter_Is_Active_Should_Return_Only_Active_Todos()
    {
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Active 1"));
        await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Active 2"));

        var completedTodoResponse = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Completed"));
        var completedTodo = await completedTodoResponse.Content.ReadFromJsonAsync<TodoResponse>();
        await _client.PatchAsync($"/api/todo/{completedTodo!.Id}/complete", null);

        var response = await _client.GetAsync("/api/todo?filter=active");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Should().NotBeEmpty();
        todos.Should().OnlyContain(t => !t.IsCompleted);
    }

    [Fact]
    public async Task GetTodos_When_Filter_Is_Completed_Should_Return_Only_Completed_Todos()
    {
        var activeTodoResponse = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Active"));
        var activeTodo = await activeTodoResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var completedTodoResponse = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Completed"));
        var completedTodo = await completedTodoResponse.Content.ReadFromJsonAsync<TodoResponse>();
        await _client.PatchAsync($"/api/todo/{completedTodo!.Id}/complete", null);

        (await _client.GetAsync($"/api/todo?filter=active")).StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.GetAsync("/api/todo?filter=completed");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();
        todos.Should().NotBeNull();
        todos!.Should().NotBeEmpty();
        todos.Should().OnlyContain(t => t.IsCompleted);
        todos.Should().Contain(t => t.Id == completedTodo.Id);
    }

    [Fact]
    public async Task UpdateTodo_When_Id_NotFound_Should_Return_NotFound()
    {
        var updateRequest = new UpdateTodoRequest("Castelvania");
        var id = Guid.NewGuid();

        var response = await _client.PutAsJsonAsync($"/api/todo/{id}", updateRequest);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateTodo_When_Name_Empty_Should_Return_BadRequest()
    {
        var createRequest = new CreateTodoRequest("");
        var response = await _client.PostAsJsonAsync("/api/todo", createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MarkAsCompleted_When_Id_NotFound_Should_Return_NotFound()
    {
        var id = Guid.NewGuid();

        var response = await _client.PatchAsync($"/api/todo/{id}/complete", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task MarkAsNotCompleted_When_Id_NotFound_Should_Return_NotFound()
    {
        var id = Guid.NewGuid();

        var response = await _client.PatchAsync($"/api/todo/{id}/incomplete", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.Should().NotBeNull();
        error!.error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Delete_When_Id_NotFound_Should_Return_NoContent()
    {
        var id = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/todo/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Create_When_Valid_Should_Return_CreatedTodo_And_NotCompleted()
    {
        var createRequest = new CreateTodoRequest("Valid Todo");

        var response = await _client.PostAsJsonAsync("/api/todo", createRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoResponse>();
        createdTodo.Should().NotBeNull();
        createdTodo!.Name.Should().Be(createRequest.Name);
        createdTodo.IsCompleted.Should().BeFalse();
        createdTodo.UpdatedAt.Should().BeNull();
    }
}


