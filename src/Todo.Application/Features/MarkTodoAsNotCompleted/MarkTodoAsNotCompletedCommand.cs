using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;

namespace Todo.Application.Features.MarkTodoAsNotCompleted;

public record MarkTodoAsNotCompletedCommand(Guid Id) : IRequest<TodoResponse>;

