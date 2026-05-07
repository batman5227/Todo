using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Features.DeleteCompletedTodos;
using Todo.Domain.Interfaces;
using Xunit;

namespace Todo.Application.UnitTests.Features.DeleteCompletedTodos;

public class DeleteCompletedTodosHandlerTests
{
    [Fact]
    public async Task Handle_ShouldDeleteAllCompletedTodos()
    {
        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.DeleteAllCompletedAsync()).Returns(Task.CompletedTask);

        var handler = new DeleteCompletedTodosHandler(repo.Object);

        var result = await handler.Handle(new DeleteCompletedTodosCommand(), CancellationToken.None);

        result.Should().NotBeNull();
        repo.Verify(r => r.DeleteAllCompletedAsync(), Times.Once);
    }
}

