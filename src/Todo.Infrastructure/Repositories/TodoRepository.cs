using Microsoft.EntityFrameworkCore;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Todo.Infrastructure.Data;

namespace Todo.Infrastructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly AppDbContext _context;

        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync(bool? isCompleted = null)
        {
            var query = _context.Todos.AsQueryable();
            
            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);
            
            return await query.ToListAsync();
        }

        public async Task<TodoItem?> GetByIdAsync(Guid id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task AddAsync(TodoItem todo)
        {
            await _context.Todos.AddAsync(todo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TodoItem todo)
        {
            _context.Entry(todo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodoItem todo)
        {
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllCompletedAsync()
        {
            var completedTodos = await _context.Todos
                .Where(t => t.IsCompleted)
                .ToListAsync();
            
            _context.Todos.RemoveRange(completedTodos);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAllStatusAsync(bool isCompleted)
        {
            var allTodos = await _context.Todos.ToListAsync();
            
            foreach (var todo in allTodos)
            {
                if (isCompleted)
                    todo.MarkAsCompleted();
                else
                    todo.MarkAsNotCompleted();
            }
            
            await _context.SaveChangesAsync();
        }
    }
}