using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;

namespace Todo.Application.Features.UpdateTodo;

public record UpdateTodoCommand(Guid Id) : IRequest<TodoResponse>;

