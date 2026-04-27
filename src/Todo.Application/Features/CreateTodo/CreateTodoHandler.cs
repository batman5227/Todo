using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Features.CreateTodo;

public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, TodoResponse>
{
    private readonly ITodoRepository _repository;

    public CreateTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new TodoItem(request.Name);
        await _repository.AddAsync(todo);

        return new TodoResponse(
            todo.Id,
            todo.Name,
            todo.IsCompleted,
            todo.CreatedAt,
            todo.UpdatedAt
        );
    }
}

