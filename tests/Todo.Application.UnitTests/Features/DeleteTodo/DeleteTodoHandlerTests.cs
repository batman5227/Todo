using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Common.Mediator;
using Todo.Application.Features.DeleteTodo;
using Todo.Application.Features.MarkTodoAsCompleted;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.DeleteTodo;

public class DeleteTodoHandlerTests
{
    [Fact]
    public async Task Handle_WhenTodoExists_ShouldDelete()
    {
        var id = Guid.NewGuid();
        var todo = TodoItem.Create("Todo");

        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(todo);

        var handler = new DeleteTodoHandler(repo.Object);

        var result = await handler.Handle(new DeleteTodoCommand(id), CancellationToken.None);

        result.Should().NotBeNull();
        repo.Verify(r => r.DeleteAsync(todo), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTodoDoesNotExist_ShouldNotDelete()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TodoItem?)null);

        var handler = new DeleteTodoHandler(repo.Object);

        await handler.Handle(new DeleteTodoCommand(id), CancellationToken.None);

        repo.Verify(r => r.DeleteAsync(It.IsAny<TodoItem>()), Times.Never);
    }
}

