using System;
using FluentAssertions;
using Todo.Application.Features.UpdateTodo;
using Xunit;

namespace Todo.Application.UnitTests.Features.UpdateTodo;

public class UpdateTodoValidatorTests
{
    private readonly UpdateTodoValidator _validator;

    public UpdateTodoValidatorTests()
    {
        _validator = new UpdateTodoValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        
        var command = new UpdateTodoCommand(Guid.NewGuid(), "Valeur valide");

        
        var result = _validator.Validate(command);

        
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyId_ShouldHaveError()
    {

        var command = new UpdateTodoCommand(Guid.Empty, "Valeur valide");

       
        var result = _validator.Validate(command);

        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
