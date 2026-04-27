using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;

namespace Todo.Application.Features.CreateTodo;

public record CreateTodoCommand(string Name) : IRequest<TodoResponse>;

