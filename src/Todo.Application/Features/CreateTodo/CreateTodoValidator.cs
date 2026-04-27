using FluentValidation;

namespace Todo.Application.Features.CreateTodo;

public class CreateTodoValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom ne peut pas être vide")
            .MinimumLength(2).WithMessage("Le nom doit contenir au minimum 2 caractères")
            .MaximumLength(100).WithMessage("Le nom doit contenir au maximum 100 caractères");
    }
}

