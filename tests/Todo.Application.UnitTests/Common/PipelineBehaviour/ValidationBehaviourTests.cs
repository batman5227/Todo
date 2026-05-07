using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Todo.Application.Common.Mediator;
using Todo.Application.Common.PipelineBehaviour;
using Todo.Application.DTOs;
using Xunit;

namespace Todo.Application.UnitTests.Common.PipelineBehaviour;

public class ValidationBehaviourTests
{
    private sealed class FakeRequest : IRequest<TodoResponse> { }

    private sealed class FakeValidator : IValidator<FakeRequest>
    {
        private readonly bool _shouldFail;
        public FakeValidator(bool shouldFail) => _shouldFail = shouldFail;

        public ValidationResult Validate(FakeRequest instance)
        {
            if (!_shouldFail) return new ValidationResult();
            return new ValidationResult(new[]
            {
                new ValidationFailure("x", "y")
            });
        }

        public Task<ValidationResult> ValidateAsync(FakeRequest instance, CancellationToken cancellation = default)
            => Task.FromResult(Validate(instance));

        public ValidationResult Validate(FluentValidation.IValidationContext context)
        {
            if (context is FluentValidation.ValidationContext<FakeRequest> typed)
                return Validate(typed.InstanceToValidate);

            return new ValidationResult();
        }

        public Task<ValidationResult> ValidateAsync(FluentValidation.IValidationContext context, CancellationToken cancellation = default)
            => Task.FromResult(Validate(context));

        public IValidatorDescriptor CreateDescriptor() => null!;
        public bool CanValidateInstancesOfType(System.Type type) => true;
    }

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldInvokeNext()
    {
        var behaviour = new ValidationBehaviour<FakeRequest, TodoResponse>(new IValidator<FakeRequest>[0]);

        var nextCalled = false;

        Task<TodoResponse> Next()
        {
            nextCalled = true;
            return Task.FromResult(new TodoResponse(System.Guid.NewGuid(), "n", false, System.DateTime.UtcNow, null));
        }

        var result = await behaviour.Handle(new FakeRequest(), CancellationToken.None, Next);

        nextCalled.Should().BeTrue();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        var behaviour = new ValidationBehaviour<FakeRequest, TodoResponse>(new[] { new FakeValidator(true) });

        Task<TodoResponse> Next() =>
            Task.FromResult(new TodoResponse(System.Guid.NewGuid(), "n", false, System.DateTime.UtcNow, null));

        var act = () => behaviour.Handle(new FakeRequest(), CancellationToken.None, Next);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_WhenValidationSucceeds_ShouldInvokeNext()
    {
        var behaviour = new ValidationBehaviour<FakeRequest, TodoResponse>(new[] { new FakeValidator(false) });

        var nextCalled = false;

        Task<TodoResponse> Next()
        {
            nextCalled = true;
            return Task.FromResult(new TodoResponse(System.Guid.NewGuid(), "n", false, System.DateTime.UtcNow, null));
        }

        var result = await behaviour.Handle(new FakeRequest(), CancellationToken.None, Next);

        nextCalled.Should().BeTrue();
        result.Should().NotBeNull();
    }
}
