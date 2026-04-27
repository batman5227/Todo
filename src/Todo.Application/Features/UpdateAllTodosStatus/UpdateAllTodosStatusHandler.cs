using Todo.Application.Common.Mediator;
using Todo.Domain.Interfaces;

namespace Todo.Application.Features.UpdateAllTodosStatus;

public class UpdateAllTodosStatusHandler : IRequestHandler<UpdateAllTodosStatusCommand, Unit>
{
    private readonly ITodoRepository _repository;

    public UpdateAllTodosStatusHandler(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateAllTodosStatusCommand request, CancellationToken cancellationToken)
    {
        await _repository.UpdateAllStatusAsync(request.IsCompleted);
        return Unit.Value;
    }
}

