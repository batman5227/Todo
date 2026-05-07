using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Todo.Application.Common.Mediator;
using Todo.Application.Features.UpdateTodo;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.UpdateTodo;

public class UpdateTodoHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnTodo()
    {

        var todoId = Guid.NewGuid();
        var todoExistant = CreateTodoWithId("Nom existant", todoId);
        var command = new UpdateTodoCommand(todoId, "Nouveau nom");

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoId))
                      .ReturnsAsync(todoExistant);

        var mockMediator = new Mock<IMediator>();
        var handler = new UpdateTodoHandler(mockRepository.Object, mockMediator.Object);

        
        var result = await handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Nom existant");
        result.Id.Should().Be(todoId);
        
        mockRepository.Verify(r => r.GetByIdAsync(todoId), Times.Once);

    }

    [Fact]
    public async Task Handle_WithNonExistentTodo_ShouldThrowKeyNotFoundException()
    {
        
        var todoId = Guid.NewGuid();
        var command = new UpdateTodoCommand(todoId, "Nouveau nom");

        var mockRepository = new Mock<ITodoRepository>();
        mockRepository.Setup(r => r.GetByIdAsync(todoId))
                      .ReturnsAsync((TodoItem?)null);

        var mockMediator = new Mock<IMediator>();
        var handler = new UpdateTodoHandler(mockRepository.Object, mockMediator.Object);

        
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Todo avec l'ID {todoId} non trouvé");
    }

   
        private TodoItem CreateTodoWithId(string name, Guid id)
    {
        var todo = TodoItem.Create(name);
        var field = typeof(TodoItem).GetField("<Id>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(todo, id);
        return todo;
    }

}
