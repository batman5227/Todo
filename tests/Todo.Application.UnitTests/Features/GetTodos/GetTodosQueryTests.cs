using FluentAssertions;
using Todo.Application.Features.GetTodos;
using Xunit;
namespace Todo.Application.UnitTests.Features.GetTodos;
public class GetTodosQueryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    public void Query_Should_Return_GetTodosQuery( bool? isCompleted)
    {
        var query = new GetTodosQuery(isCompleted);

        query.Should().BeOfType<GetTodosQuery>();
    }
}