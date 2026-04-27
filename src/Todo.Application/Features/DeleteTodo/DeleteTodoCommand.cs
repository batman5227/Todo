using Todo.Application.Common.Mediator;

namespace Todo.Application.Features.DeleteTodo;

public record DeleteTodoCommand(Guid Id) : IRequest<Unit>;

