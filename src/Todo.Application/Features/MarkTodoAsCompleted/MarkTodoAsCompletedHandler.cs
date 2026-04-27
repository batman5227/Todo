using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.MarkTodoAsCompleted;

public class MarkTodoAsCompletedHandler : IRequestHandler<MarkTodoAsCompletedCommand, TodoResponse>
{
    private readonly ITodoRepository _repository;

    public MarkTodoAsCompletedHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoResponse> Handle(MarkTodoAsCompletedCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        if (todo == null)
            throw new KeyNotFoundException($"Todo avec l'ID {request.Id} non trouvé");

        todo.MarkAsCompleted();
        await _repository.UpdateAsync(todo);

        return new TodoResponse(
            todo.Id,
            todo.Name,
            todo.IsCompleted,
            todo.CreatedAt,
            todo.UpdatedAt
        );
    }
}

