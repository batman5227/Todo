
using Moq;
using FluentAssertions;
using Todo.Application.Common.Mediator;
using Todo.Application.Features.UpdateAllTodosStatus;
using Todo.Domain.Interfaces;

namespace Todo.Application.UnitTests.Features.UpdateAllTodosStatus;

public class UpdateAllTodosStatusHandlerTests
{
    [Fact]
    public async Task Handler_Should_Update_All_Todos_Status()
    {
        var isCompleted = true;

        var mockRepository = new Mock<ITodoRepository>();
        var handler = new UpdateAllTodosStatusHandler(mockRepository.Object);

        Unit result = await handler.Handle(new UpdateAllTodosStatusCommand(isCompleted), CancellationToken.None);

        result.Should().Be(Unit.Value);
        mockRepository.Verify(r => r.UpdateAllStatusAsync(isCompleted), Times.Once);
    }
}


