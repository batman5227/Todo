using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;

namespace Todo.Application.Features.MarkTodoAsCompleted;

public record MarkTodoAsCompletedCommand(Guid Id) : IRequest<TodoResponse>;

