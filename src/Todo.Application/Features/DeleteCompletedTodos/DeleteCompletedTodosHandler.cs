using Todo.Application.Common.Mediator;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.DeleteCompletedTodos;

public class DeleteCompletedTodosHandler : IRequestHandler<DeleteCompletedTodosCommand, Unit>
{
    private readonly ITodoRepository _repository;

    public DeleteCompletedTodosHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCompletedTodosCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAllCompletedAsync();
        return Unit.Value;
    }
}

