using FluentAssertions;
using Todo.Application.Features.CreateTodo;


namespace Todo.Application.UnitTests.Features.CreateTodo;
public class CreateCommandTests
{
    [Fact]
    public void Command_Should_Contain_Name()
    {
        
        var name = "Test Todo";

        
        var command = new CreateTodoCommand(name);

        
        command.Name.Should().Be(name);
    }

    [Fact]
    public void Command_WithEmptyName_Should_Allow_Empty_Name()
    {
        
        var command = new CreateTodoCommand(string.Empty);

        
        command.Name.Should().BeEmpty();
    }

    [Fact]
    public void Command_WithWhitespaceName_Should_Allow_Whitespace()
    {
        
        var command = new CreateTodoCommand("   ");

        
        command.Name.Should().Be("   ");
    }
}
