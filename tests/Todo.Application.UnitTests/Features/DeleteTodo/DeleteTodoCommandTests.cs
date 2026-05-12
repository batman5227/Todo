using FluentAssertions;
using Todo.Application.Features.DeleteCompletedTodos;
using Todo.Application.Features.DeleteTodo;

namespace Todo.Application.UnitTests.Features.DeleteTodo;

public class DeleteTodoCommandTests
{
    [Fact]
    public void Command_Should_Contain_Id()
    {
        
        var todoId = Guid.NewGuid();

        
        var command = new DeleteTodoCommand(todoId);

        
        command.Id.Should().Be(todoId);
    }

    [Fact]
    public void Command_WithEmptyId_Should_Allow_EmptyId()
    {
        
        var command = new DeleteTodoCommand(Guid.Empty);

        
        command.Id.Should().Be(Guid.Empty);
    }
}