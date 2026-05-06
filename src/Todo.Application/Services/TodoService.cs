using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;

namespace Todo.Application.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoResponse>> GetAllTodosAsync(bool? isCompleted = null)
        {
            var todos = await _repository.GetAllAsync(isCompleted);

            var orderedTodos = todos.OrderBy(t => t.CreatedAt);

            return orderedTodos.Select(t => new TodoResponse(
                t.Id,
                t.Name,
                t.IsCompleted,
                t.CreatedAt,
                t.UpdatedAt
            ));
        }

        public async Task<TodoResponse> CreateTodoAsync(string name)
        {
            var todo =TodoItem.Create(name);
            await _repository.AddAsync(todo);

            return new TodoResponse(
                todo.Id,
                todo.Name,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.UpdatedAt
            );
        }

        public async Task<TodoResponse> UpdateTodoAsync(Guid id, string name)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                throw new KeyNotFoundException($"Todo avec l'ID {id} non trouvé");

            todo.SetName(name);
            await _repository.UpdateAsync(todo);

            return new TodoResponse(
                todo.Id,
                todo.Name,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.UpdatedAt
            );
        }

        public async Task<TodoResponse> MarkAsCompletedAsync(Guid id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                throw new KeyNotFoundException($"Todo avec l'ID {id} non trouvé");

            todo.MarkAsCompleted();
            await _repository.UpdateAsync(todo);

            return new TodoResponse(
                todo.Id,
                todo.Name,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.UpdatedAt
            );
        }

        public async Task<TodoResponse> MarkAsNotCompletedAsync(Guid id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                throw new KeyNotFoundException($"Todo avec l'ID {id} non trouvé");

            todo.MarkAsNotCompleted();
            await _repository.UpdateAsync(todo);

            return new TodoResponse(
                todo.Id,
                todo.Name,
                todo.IsCompleted,
                todo.CreatedAt,
                todo.UpdatedAt
            );
        }

        public async Task DeleteTodoAsync(Guid id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo != null)
                await _repository.DeleteAsync(todo);
        }

        public async Task DeleteAllCompletedAsync()
        {
            await _repository.DeleteAllCompletedAsync();
        }

        public async Task UpdateAllStatusAsync(bool isCompleted)
        {
            await _repository.UpdateAllStatusAsync(isCompleted);
        }
    }
}
