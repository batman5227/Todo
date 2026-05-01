using System;
using FluentAssertions;
using Todo.Application.Features.UpdateTodo;
using Xunit;

namespace Todo.Application.UnitTests.Features.UpdateTodo;

public class UpdateTodoCommandTests
{
    [Fact]
    public void Command_ShouldContainIdAndValue()
    {
        
        var todoId = Guid.NewGuid();
        var value = "Nouveau nom";

        
        var command = new UpdateTodoCommand(todoId, value);

        
        command.Id.Should().Be(todoId);
        command.value.Should().Be(value);
    }

    [Fact]
    public void Command_WithEmptyId_ShouldAllowEmptyId()
    {
       
        var command = new UpdateTodoCommand(Guid.Empty, "Valeur valide");

        
        command.Id.Should().Be(Guid.Empty);
        command.value.Should().Be("Valeur valide");
    }

    [Fact]
    public void Command_WithNullValue_ShouldAllowNull()
    {
        
        var command = new UpdateTodoCommand(Guid.NewGuid(), null!);

        
        command.value.Should().BeNull();
    }

    [Fact]
    public void Command_Properties_ShouldBeAccessible()
    {
        
        var todoId = Guid.NewGuid();
        var value = "Test valeur";

        
        var command = new UpdateTodoCommand(todoId, value);

        
        Assert.Equal(todoId, command.Id);
        Assert.Equal(value, command.value);
    }
}
