using FluentAssertions;
using Todo.Application.Features.UpdateAllTodosStatus;

namespace Todo.Application.UnitTests.Features.UpdateAllTodosStatus;

public class UpdateAllTodosStatusCommandTests
{    [Fact]
    public void Command_Should_Contain_IsCompleted()
    {
        
        var isCompleted = true;

        
        var command = new UpdateAllTodosStatusCommand(isCompleted);

        
        command.IsCompleted.Should().Be(isCompleted);
    }
}