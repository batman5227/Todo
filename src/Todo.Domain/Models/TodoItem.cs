using System;
using System.Runtime.InteropServices;

namespace Todo.Domain.Models
{
    public class TodoItem
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; set; }

        private TodoItem()
        {
          
        }

public static TodoItem Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom ne peut pas être vide");
            if (name.Length < 3)
                throw new ArgumentException("Le nom doit contenir au minimum 3 caractères");
            if (name.Length > 100)
                throw new ArgumentException("Le nom doit contenir au maximum 100 caractères");
            return new TodoItem
            {
                Id = Guid.NewGuid(),
                Name = name.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };
        }
        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Le nom ne peut pas être vide");
            
            if (name.Length < 2)
                throw new ArgumentException("Le nom doit contenir au minimum 2 caractères");
            
            if (name.Length > 100)
                throw new ArgumentException("Le nom doit contenir au maximum 100 caractères");
            
            Name = name.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsNotCompleted()
        {
            IsCompleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ToggleComplete()
        {
            IsCompleted = !IsCompleted;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}