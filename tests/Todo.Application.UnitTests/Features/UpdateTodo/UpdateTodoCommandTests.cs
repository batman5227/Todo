using System;
using FluentAssertions;
using Todo.Application.Features.UpdateTodo;
using Xunit;

namespace Todo.Application.UnitTests.Features.UpdateTodo;

public class UpdateTodoCommandTests
{
    [Fact]
    public void Command_Should_Contain_IdAndValue()
    {
        
        var todoId = Guid.NewGuid();
        var value = "Nouveau nom";

        
        var command = new UpdateTodoCommand(todoId, value);

        
        command.Id.Should().Be(todoId);
        command.value.Should().Be(value);
    }

    [Fact]
    public void Command_WithEmptyId_Should_Allow_EmptyId()
    {
       
        var command = new UpdateTodoCommand(Guid.Empty, "Valeur valide");

        
        command.Id.Should().Be(Guid.Empty);
        command.value.Should().Be("Valeur valide");
    }

    [Fact]
    public void Command_With_Null_Value_Should_Allow_Null()
    {
        
        var command = new UpdateTodoCommand(Guid.NewGuid(), null!);

        
        command.value.Should().BeNull();
    }

    [Fact]
    public void Command_Properties_Should_Be_Accessible()
    {
        
        var todoId = Guid.NewGuid();
        var value = "Test valeur";

        
        var command = new UpdateTodoCommand(todoId, value);

        
        Assert.Equal(todoId, command.Id);
        Assert.Equal(value, command.value);
    }

    [Fact]
    public void Command_With_Whitespace_Value_Should_Allow_Whitespace()
    {
        
        var command = new UpdateTodoCommand(Guid.NewGuid(), "   ");

        
        command.value.Should().Be("   ");
    }

        [Fact]
        public void Update_Name_Should_Be_Updated()
        {
            
            var todoId = Guid.NewGuid();
            var value = "Nouveau nom";

            
            var command = new UpdateTodoCommand(todoId, value);

            
            command.value.Should().Be(value);
        }
}
