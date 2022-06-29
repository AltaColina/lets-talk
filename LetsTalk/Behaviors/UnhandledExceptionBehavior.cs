using MediatR;

namespace LetsTalk.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    async Task<TResponse> IPipelineBehavior<TRequest, TResponse>.Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request: Unhandled exception for request {Name} {@Request}", typeof(TRequest).Name, request);

            throw;
        }
    }
}
