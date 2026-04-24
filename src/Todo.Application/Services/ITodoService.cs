using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Application.DTOs;

namespace Todo.Application.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoResponse>> GetAllTodosAsync(bool? isCompleted = null);
        Task<TodoResponse> CreateTodoAsync(string name);
        Task<TodoResponse> UpdateTodoAsync(Guid id, string name);
        Task<TodoResponse> MarkAsCompletedAsync(Guid id);
        Task<TodoResponse> MarkAsNotCompletedAsync(Guid id);
        Task DeleteTodoAsync(Guid id);
        Task DeleteAllCompletedAsync();
        Task UpdateAllStatusAsync(bool isCompleted);
    }
}