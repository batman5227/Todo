using Todo.Application.Features.MarkTodoAsCompleted;
using FluentAssertions;

namespace Todo.Application.UnitTests.Features.MarkTodoAsCompleted;

public class MarkTodoAsCompletedCommandTests
{
    [Fact]
    public void Command_Should_Contain_Id()
    {
        
        var todoId = Guid.NewGuid();

        
        var command = new MarkTodoAsCompletedCommand(todoId);

        
        command.Id.Should().Be(todoId);
    }

    [Fact]
    public void Command_WithEmptyId_Should_Allow_EmptyId()
    {
        
        var command = new MarkTodoAsCompletedCommand(Guid.Empty);

        
        command.Id.Should().Be(Guid.Empty);
    }
}