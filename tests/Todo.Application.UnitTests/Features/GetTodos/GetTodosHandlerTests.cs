using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Features.GetTodos;
using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.GetTodos;

public class GetTodosHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrderedMappedTodos()
    {
        var repo = new Mock<ITodoRepository>();
        var mediator = new Mock<IMediator>();

        var older = TodoItem.Create("Older");
        var newer = TodoItem.Create("Newer");

      
        var createdAtField = typeof(TodoItem).GetProperty("CreatedAt");
        createdAtField.Should().NotBeNull();

        await Task.Delay(10);
        var evenNewer = TodoItem.Create("EvenNewer");

     
        var list = new List<TodoItem> { evenNewer, older, newer };

        repo.Setup(r => r.GetAllAsync(null))
            .ReturnsAsync(list);

        var handler = new GetTodosHandler(repo.Object);

        var result = (await handler.Handle(new GetTodosQuery(null), CancellationToken.None)).ToList();

        result.Should().HaveCount(3);
        result.Select(r => r.Name).Should().Equal(list.OrderBy(t => t.CreatedAt).Select(t => t.Name));

        result.All(r => r is TodoResponse).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
    {
        var repo = new Mock<ITodoRepository>();
        repo.Setup(r => r.GetAllAsync(false)).ReturnsAsync(Array.Empty<TodoItem>());

        var handler = new GetTodosHandler(repo.Object);

        var result = await handler.Handle(new GetTodosQuery(false), CancellationToken.None);

        result.Should().BeEmpty();
    }
}

