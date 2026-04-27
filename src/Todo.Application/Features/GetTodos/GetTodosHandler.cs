using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.GetTodos;

public class GetTodosHandler : IRequestHandler<GetTodosQuery, IEnumerable<TodoResponse>>
{
    private readonly ITodoRepository _repository;

    public GetTodosHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoResponse>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var todos = await _repository.GetAllAsync(request.IsCompleted);

        return todos
            .OrderBy(t => t.CreatedAt)
            .Select(t => new TodoResponse(
                t.Id,
                t.Name,
                t.IsCompleted,
                t.CreatedAt,
                t.UpdatedAt
            ));
    }
}

