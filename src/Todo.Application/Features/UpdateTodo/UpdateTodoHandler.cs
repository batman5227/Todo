using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.UpdateTodo;

public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, TodoResponse>
{
    private readonly ITodoRepository _repository;

    public UpdateTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoResponse> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        if (todo == null)
            throw new KeyNotFoundException($"Todo avec l'ID {request.Id} non trouvé");

        return new TodoResponse(
            todo.Id,
            todo.Name,
            todo.IsCompleted,
            todo.CreatedAt,
            todo.UpdatedAt
        );
    }
}

