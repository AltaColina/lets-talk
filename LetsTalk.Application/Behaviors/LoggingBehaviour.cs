using MediatR;
using Microsoft.Extensions.Logging;

namespace LetsTalk.Behaviors;

public sealed class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request: {Type} {@Request}", typeof(TRequest), request);
        var response = await next();
        _logger.LogInformation("Response: {Type} {@Request}", typeof(TResponse), response);
        return response;
    }
}
