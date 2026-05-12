
using Moq;
using FluentAssertions;
using Todo.Application.DTOs;
using Todo.Application.Features.MarkTodoAsNotCompleted;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.UnitTests.Features.MarkTodoAsNotCompleted;

public class MarkTodoAsNotCompletedHandlerTests
{
    [Fact]
    public async Task Handler_Should_Mark_Todo_As_Not_Completed()
    {
        var todoId = Guid.NewGuid();

        var todo = TodoItem.Create("Test");
        todo.MarkAsCompleted();


        var mockRepository = new Mock<ITodoRepository>();
        mockRepository
            .Setup(r => r.GetByIdAsync(todoId))
            .ReturnsAsync(todo);

        var handler = new MarkTodoAsNotCompletedHandler(mockRepository.Object);

        TodoResponse result = await handler.Handle(new MarkTodoAsNotCompletedCommand(todoId), CancellationToken.None);

        result.IsCompleted.Should().BeFalse();

        mockRepository.Verify(r => r.GetByIdAsync(todoId), Times.Once);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        mockRepository.Verify(r => r.MarkAsNotCompletedAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task Handler_With_NonExistent_Todo_Should_Throw_KeyNotFoundException()
    {
        var todoId = Guid.NewGuid();

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync((TodoItem?)null);

        var handler = new MarkTodoAsNotCompletedHandler(mockRepository.Object);

        Func<Task> act = () => handler.Handle(new MarkTodoAsNotCompletedCommand(todoId), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Never);
    }
}


