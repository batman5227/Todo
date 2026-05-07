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
public async Task GetTodos_Should_Return_All_Todos_When_IsCompleted_Is_Null()
{
    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Gurenn Lagann"));

    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Black Clover"));

    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Inuyasha"));

    var response =
        await _client.GetAsync("/api/todo");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var todos =
        await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

    todos.Should().NotBeNull();

    todos.Should().Contain(t => t.Name == "Gurenn Lagann");
    todos.Should().Contain(t => t.Name == "Black Clover");
    todos.Should().Contain(t => t.Name == "Inuyasha");
}

 [Fact]
public async Task GetTodos_Should_Return_All_Todos()
{
    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Gurenn Lagann"));

    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Black Clover"));

    await _client.PostAsJsonAsync(
        "/api/todo",
        new CreateTodoRequest("Inuyasha"));


    var response = await _client.GetAsync("/api/todo");


    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var todos =
        await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

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
    var r1 = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Black Star"));
    var r2 = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Shinra Kusakabe"));
    var r3 = await _client.PostAsJsonAsync("/api/todo", new CreateTodoRequest("Arthur Boyle"));

    var t1 = await r1.Content.ReadFromJsonAsync<TodoResponse>();
    var t2 = await r2.Content.ReadFromJsonAsync<TodoResponse>();
    var t3 = await r3.Content.ReadFromJsonAsync<TodoResponse>();

    var updateRequest = new UpdateAllStatusRequest(true);

    await _client.PatchAsJsonAsync(
        "/api/todo/update-all",
        updateRequest);

    var beforeDeleteResponse =
        await _client.GetAsync("/api/todo?isCompleted=true");

    var completedBefore =
        await beforeDeleteResponse.Content.ReadFromJsonAsync<List<TodoResponse>>();

    completedBefore.Should().NotBeNull();
 
    var deleteResponse =
        await _client.DeleteAsync("/api/todo/completed");

    deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var afterCompleted =
        await _client.GetAsync("/api/todo?isCompleted=true");

    var completedAfter =
        await afterCompleted.Content.ReadFromJsonAsync<List<TodoResponse>>();

    completedAfter.Should().NotBeNull();
    completedAfter!.Should().BeEmpty();

    var allTodosResponse =
        await _client.GetAsync("/api/todo");

    var allTodos =
        await allTodosResponse.Content.ReadFromJsonAsync<List<TodoResponse>>();

    allTodos.Should().NotBeNull();

    allTodos!.Should().BeEmpty();
}


 [Fact]
public async Task UpdateAllStatus_Should_Update_All_Todos_To_Completed_Or_NotCompleted()
{

    var createRequest1 = new CreateTodoRequest("Tokyo Ghoul");
    var createRequest2 = new CreateTodoRequest("Demon Slayer");
    var createRequest3 = new CreateTodoRequest("One Piece");

    var r1 = await _client.PostAsJsonAsync("/api/todo", createRequest1);
    var r2 = await _client.PostAsJsonAsync("/api/todo", createRequest2);
    var r3 = await _client.PostAsJsonAsync("/api/todo", createRequest3);

    var t1 = await r1.Content.ReadFromJsonAsync<TodoResponse>();
    var t2 = await r2.Content.ReadFromJsonAsync<TodoResponse>();
    var t3 = await r3.Content.ReadFromJsonAsync<TodoResponse>();


    var updateRequest = new UpdateAllStatusRequest(true);

    var updateResponse =
        await _client.PatchAsJsonAsync("/api/todo/update-all", updateRequest);


    updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

    var response = await _client.GetAsync("/api/todo");

    response.StatusCode.Should().Be(HttpStatusCode.OK);

    var todos =
        await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

   
    todos.Should().NotBeNull();
    todos.Should().OnlyContain(t => t.IsCompleted);
    todos.Should().Contain(t => t.Id == t1!.Id && t.IsCompleted);
    todos.Should().Contain(t => t.Id == t2!.Id && t.IsCompleted);
    todos.Should().Contain(t => t.Id == t3!.Id && t.IsCompleted);

    var updateRequestFalse = new UpdateAllStatusRequest(false);

    var updateResponseFalse =
        await _client.PatchAsJsonAsync("/api/todo/update-all", updateRequestFalse);

    updateResponseFalse.StatusCode.Should().Be(HttpStatusCode.OK);

    var responseAfter =
        await _client.GetAsync("/api/todo?isCompleted=false");

    var notCompleted =
        await responseAfter.Content.ReadFromJsonAsync<List<TodoResponse>>();

    notCompleted.Should().NotBeNull();
    notCompleted.Should().OnlyContain(t => !t.IsCompleted);
}
}