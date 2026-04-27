using Todo.Application.Common.Mediator;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.DeleteTodo;

public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, Unit>
{
    private readonly ITodoRepository _repository;

    public DeleteTodoHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _repository.GetByIdAsync(request.Id);
        if (todo != null)
            await _repository.DeleteAsync(todo);

        return Unit.Value;
    }
}

