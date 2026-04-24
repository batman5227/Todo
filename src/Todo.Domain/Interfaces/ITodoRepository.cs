using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Domain.Models;

namespace Todo.Domain.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllAsync(bool? isCompleted = null);
        Task<TodoItem?> GetByIdAsync(Guid id);
        Task AddAsync(TodoItem todo);
        Task UpdateAsync(TodoItem todo);
        Task DeleteAsync(TodoItem todo);
        Task DeleteAllCompletedAsync();
        Task UpdateAllStatusAsync(bool isCompleted);
    }
}