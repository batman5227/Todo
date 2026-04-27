namespace Todo.Application.Common.Mediator;

public interface IPipelineBehaviour<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<Task<TResponse>> next);
}

