using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Features.MarkTodoAsCompleted;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.MarkTodoAsCompleted;

public class MarkTodoAsCompletedHandlerTests
{
    [Fact]
    public async Task Handle_WhenTodoExists_ShouldMarkCompleted()
    {
        var id = Guid.NewGuid();
        var todo = TodoItem.Create("Todo");

        var idField = typeof(TodoItem).GetField("<Id>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        idField!.SetValue(todo, id);

        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(todo);

        var handler = new MarkTodoAsCompletedHandler(repo.Object);

        TodoResponse result = await handler.Handle(new MarkTodoAsCompletedCommand(id), CancellationToken.None);

        result.IsCompleted.Should().BeTrue();
        repo.Verify(r => r.UpdateAsync(todo), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenTodoDoesNotExist_ShouldThrowKeyNotFoundException()
    {
        var id = Guid.NewGuid();

        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TodoItem?)null);

        var handler = new MarkTodoAsCompletedHandler(repo.Object);

        Func<Task> act = async () => await handler.Handle(new MarkTodoAsCompletedCommand(id), CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();

    }
}

