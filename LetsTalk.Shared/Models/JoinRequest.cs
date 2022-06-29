using Grpc.Core;
using MediatR;

namespace LetsTalk.Models;

public sealed partial class JoinRequest : IRequest
{
    public IServerStreamWriter<Message>? ResponseStream { get; set; }
}