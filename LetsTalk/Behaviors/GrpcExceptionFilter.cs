using Grpc.Core;
using Grpc.Core.Interceptors;
using LetsTalk.Exceptions;

namespace LetsTalk.Behaviors;

public sealed class GrpcExceptionFilter : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (UnauthorizedException ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (ForbiddenException ex)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
        }
        catch (NotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (ConflictException ex)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
        }
        catch (UnknownException ex)
        {
            throw new RpcException(new Status(StatusCode.Unknown, ex.Message));
        }
    }
}
