using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace LetsTalk.Services;

public class LetsTalkService : LetsTalk.LetsTalkBase
{
    private static readonly Person Server = new() { Username = "Server" };
    private static readonly Task<Empty> EmptyTaskResponse = Task.FromResult(new Empty());
    private static readonly ConcurrentDictionary<string, Person> LoggedUsers = new();
    private static readonly BufferBlock<Message> MessageBuffer = new(new DataflowBlockOptions { EnsureOrdered = true });
    private static readonly ConcurrentDictionary<string, IServerStreamWriter<Message>> Subscriptions = new();

    public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        var person = new Person { Username = request.Username };
        if (request.Username != Server.Username && LoggedUsers.TryAdd(request.Username, person))
            return Task.FromResult(new RegisterResponse { Person = person });
        else
            return Task.FromResult(new RegisterResponse { Person = null });
    }

    public override async Task Join(Person request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        var username = request.Username;
        Subscriptions.TryAdd(username, responseStream);

        MessageBuffer.Post(new Message { Person = Server, Text = $"{request.Username} has join the channel." });

        while (Subscriptions.ContainsKey(username))
        {
            var message = await MessageBuffer.ReceiveAsync();
            await Task.WhenAll(Subscriptions.Values.Select(s => s.WriteAsync(message)));
        }
    }

    public override Task<Empty> Send(Message request, ServerCallContext context)
    {
        MessageBuffer.Post(request);
        return EmptyTaskResponse;
    }

    public override Task<Empty> Leave(Person request, ServerCallContext context)
    {
        Subscriptions.TryRemove(request.Username, out _);
        LoggedUsers.TryRemove(request.Username, out _);
        MessageBuffer.Post(new Message { Person = Server, Text = $"{request.Username} has left the channel." });
        return EmptyTaskResponse;
    }
}
