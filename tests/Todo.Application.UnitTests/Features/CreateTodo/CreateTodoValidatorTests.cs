using Moq;
using Todo.Application.Common.Mediator;
using Todo.Application.Features.CreateTodo;
using Todo.Application.DTOs;
using Todo.Domain.Interfaces;
using Todo.Domain.Models;
using Xunit;

namespace Todo.Application.UnitTests.Features.CreateTodo;

public class CreateTodoValidatorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsInvalid_ShouldHaveValidationError(string invalidName)
    {
        var validator = new CreateTodoValidator();

        var result = validator.Validate(new CreateTodoCommand(invalidName));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WhenNameIsValid_ShouldNotHaveValidationError()
    {
        var validator = new CreateTodoValidator();

        var result = validator.Validate(new CreateTodoCommand("Valid Name"));

        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Name");
    }
}