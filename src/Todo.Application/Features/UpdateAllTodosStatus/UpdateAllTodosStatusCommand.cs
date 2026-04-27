using Todo.Application.Common.Mediator;

namespace Todo.Application.Features.UpdateAllTodosStatus;

public record UpdateAllTodosStatusCommand(bool IsCompleted) : IRequest<Unit>;

