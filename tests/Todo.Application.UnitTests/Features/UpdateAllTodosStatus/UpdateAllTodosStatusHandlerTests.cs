using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Features.UpdateAllTodosStatus;
using Todo.Domain.Interfaces;
using Xunit;

namespace Todo.Application.UnitTests.Features.UpdateAllTodosStatus;

public class UpdateAllTodosStatusHandlerTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldCallRepositoryWithRequestedIsCompleted(bool isCompleted)
    {
        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.UpdateAllStatusAsync(isCompleted)).Returns(Task.CompletedTask);

        var handler = new UpdateAllTodosStatusHandler(repo.Object);

        var result = await handler.Handle(new UpdateAllTodosStatusCommand(isCompleted), CancellationToken.None);

        result.Should().NotBeNull();
        repo.Verify(r => r.UpdateAllStatusAsync(isCompleted), Times.Once);
    }
}

