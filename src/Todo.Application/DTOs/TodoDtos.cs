using System;

namespace Todo.Application.DTOs
{
    public record CreateTodoRequest(string Name);
    
    public record UpdateTodoRequest(string Name);
    
    public record UpdateAllStatusRequest(bool IsCompleted);
    
    public record TodoResponse(
        Guid Id,
        string Name,
        bool IsCompleted,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}