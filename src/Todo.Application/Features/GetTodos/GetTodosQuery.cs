using Todo.Application.Common.Mediator;
using Todo.Application.DTOs;

namespace Todo.Application.Features.GetTodos;

public record GetTodosQuery(bool? IsCompleted) : IRequest<IEnumerable<TodoResponse>>;

