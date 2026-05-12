using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Common.Mediator;
using Todo.Application.Features.CreateTodo;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.CreateTodo;

public class CreateTodoHandlerTests
{   
    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateTodo()
    {
        var repo = new Mock<ITodoRepository>();

        var handler = new CreateTodoHandler(repo.Object);

        TodoResponse? result = await handler.Handle(new CreateTodoCommand("Valid"), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Valid");
        result.IsCompleted.Should().BeFalse();
        result.UpdatedAt.Should().BeNull();

        repo.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNameIsInvalid_ShouldThrowArgumentException()
    {
        var repo = new Mock<ITodoRepository>();
        var handler = new CreateTodoHandler(repo.Object);

        Func<Task> act = () => handler.Handle(new CreateTodoCommand(""), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
        repo.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Never);
    }
}

