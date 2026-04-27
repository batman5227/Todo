using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.MarkTodoAsNotCompleted;

public class MarkTodoAsNotCompletedHandler : IRequestHandler<MarkTodoAsNotCompletedCommand, TodoResponse>
{
    private readonly ITodoRepository _repository;

    public MarkTodoAsNotCompletedHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<TodoResponse> Handle(MarkTodoAsNotCompletedCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        if (todo == null)
            throw new KeyNotFoundException($"Todo avec l'ID {request.Id} non trouvé");

        todo.MarkAsNotCompleted();
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

