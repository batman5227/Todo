using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.DTOs;
using Todo.Application.Features.MarkTodoAsCompleted;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.MarkTodoAsCompleted;

public class MarkTodoAsCompletedHandlerTests
{
    [Fact]
    public async Task Handler_Should_Mark_Todo_As_Completed()
    {
        var todoId = Guid.NewGuid();

        var todo = TodoItem.Create("Yann");
        todo.MarkAsNotCompleted();

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoId)).ReturnsAsync(todo);

        var handler = new MarkTodoAsCompletedHandler(mockRepository.Object);

        TodoResponse result = await handler.Handle(new MarkTodoAsCompletedCommand(todoId), CancellationToken.None);

        result.IsCompleted.Should().BeTrue();

        mockRepository.Verify(r => r.GetByIdAsync(todoId), Times.Once);
        mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        mockRepository.Verify(r => r.MarkAsCompletedAsync(It.IsAny<Guid>()), Times.Never);
    }
}

