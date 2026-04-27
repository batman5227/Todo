using FluentValidation;

namespace Todo.Application.Features.UpdateTodo;

public class UpdateTodoValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("L'identifiant est requis");

    }
}

