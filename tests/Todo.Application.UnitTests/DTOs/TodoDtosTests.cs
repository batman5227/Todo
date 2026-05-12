using System;
using Todo.Application.DTOs;
using Xunit;
using FluentAssertions;

namespace Todo.UnitTests.DTOs;

public class TodoDtosTests
{
    [Fact]
    public void CreateTodoRequest_Should_Set_Name_Correctly()
    {

        var name = "One Piece";

        var request = new CreateTodoRequest(name);

        request.Name.Should().Be(name);
    }

    [Fact]
    public void UpdateTodoRequest_Should_Set_Name_Correctly()
    {
 
        var name = "Bleach";

        var request = new UpdateTodoRequest(name);

        request.Name.Should().Be(name);
    }

    [Fact]
    public void UpdateAllStatusRequest_Should_Set_IsCompleted_Correctly()
    {

        var isCompleted = true;

        var request = new UpdateAllStatusRequest(isCompleted);

        request.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void TodoResponse_Should_Set_All_Properties_Correctly()
    {

        var id = Guid.NewGuid();
        var name = "Naruto";
        var isCompleted = false;
        var createdAt = DateTime.UtcNow;
        DateTime? updatedAt = null;

        var response = new TodoResponse(
            id,
            name,
            isCompleted,
            createdAt,
            updatedAt
        );

        response.Id.Should().Be(id);
        response.Name.Should().Be(name);
        response.IsCompleted.Should().BeFalse();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void TodoResponse_Should_Accept_UpdatedAt_Value()
    {

        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var updatedAt = DateTime.UtcNow.AddMinutes(5);

        var response = new TodoResponse(
            id,
            "Dragon Ball",
            true,
            createdAt,
            updatedAt
        );


        response.Id.Should().NotBeEmpty();
        response.Name.Should().Be("Dragon Ball");
        response.IsCompleted.Should().BeTrue();
        response.CreatedAt.Should().Be(createdAt);
        response.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void CreateTodoRequest_With_Same_Values_Should_Be_Equal()
    {

        var request1 = new CreateTodoRequest("Attack on Titan");
        var request2 = new CreateTodoRequest("Attack on Titan");

        request1.Should().Be(request2);
    }

    [Fact]
    public void TodoResponse_With_Same_Values_Should_Be_Equal()
    {

        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        var todo1 = new TodoResponse(
            id,
            "Jujutsu Kaisen",
            false,
            createdAt,
            null
        );

        var todo2 = new TodoResponse(
            id,
            "Jujutsu Kaisen",
            false,
            createdAt,
            null
        );

        // Assert
        todo1.Should().Be(todo2);
    }
}