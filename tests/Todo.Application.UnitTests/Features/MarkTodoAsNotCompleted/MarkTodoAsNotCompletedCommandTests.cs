using FluentAssertions;
using Todo.Application.Features.MarkTodoAsNotCompleted;

namespace Todo.Application.UnitTests.Features.MarkTodoAsNotCompleted;

public class MarkTodoAsNotCompletedCommandTests
{
    [Fact]
    public void Command_Should_Contain_Id()
    {
        
        var todoId = Guid.NewGuid();

        
        var command = new MarkTodoAsNotCompletedCommand(todoId);

        
        command.Id.Should().Be(todoId);
    }

    [Fact]
    public void Command_WithEmptyId_Should_Allow_EmptyId()
    {
        
        var command = new MarkTodoAsNotCompletedCommand(Guid.Empty);

        
        command.Id.Should().Be(Guid.Empty);
    }
}