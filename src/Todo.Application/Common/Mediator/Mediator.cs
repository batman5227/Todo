using Microsoft.Extensions.DependencyInjection;

namespace Todo.Application.Common.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Build the pipeline
        Func<Task<TResponse>> pipeline = () => (Task<TResponse>)handlerType
            .GetMethod("Handle")!
            .Invoke(handler, new object[] { request, cancellationToken })!;

        // Pipeline behaviours
        var behaviourType = typeof(IPipelineBehaviour<,>).MakeGenericType(requestType, responseType);
        var behaviours = _serviceProvider.GetServices(behaviourType).ToList();

        foreach (var behaviour in behaviours.AsEnumerable().Reverse())
        {
            var next = pipeline;
            var handleMethod = behaviourType.GetMethod("Handle")!;
            pipeline = () => (Task<TResponse>)handleMethod.Invoke(behaviour, new object[] { request, cancellationToken, next })!;
        }

        return await pipeline();
    }
}
