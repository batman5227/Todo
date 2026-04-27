using Todo.Application.Common.Mediator;

namespace Todo.Application.Features.DeleteCompletedTodos;

public record DeleteCompletedTodosCommand : IRequest<Unit>;

