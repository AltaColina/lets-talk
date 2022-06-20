using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace LetsTalk.Services;

public class LetsTalkService : LetsTalk.LetsTalkBase
{
    private static readonly User Server = new() { Username = "Server" };
    private static readonly Task<Empty> EmptyTaskResponse = Task.FromResult(new Empty());
    private static readonly ConcurrentDictionary<string, User> RegisteredUsers = new();
    private static readonly ConcurrentDictionary<string, User> LoggedUsers = new();
    private static readonly ConcurrentDictionary<string, Chat> Chats = new();
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<Message>>> Subscriptions = new();
    private static readonly ConcurrentDictionary<string, BufferBlock<Message>> MessageBuffers = new();

    static LetsTalkService()
    {
        var chat = new Chat { Id = Guid.NewGuid().ToString(), Name = "General" };
        if (Chats.TryAdd(chat.Id, chat))
        {
            Subscriptions.TryAdd(chat.Id, new ConcurrentDictionary<string, IServerStreamWriter<Message>>());
            MessageBuffers.TryAdd(chat.Id, new BufferBlock<Message>(new DataflowBlockOptions { EnsureOrdered = true }));
        }
    }

    private static void ValidateUser(User user)
    {
        if (!LoggedUsers.ContainsKey(user.Id))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not logged in"));
    }

    private static void ValidateChannel(Chat chat)
    {
        if (!Subscriptions.ContainsKey(chat.Id))
            throw new RpcException(new Status(StatusCode.NotFound, "Chat does not exist"));
    }

    public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        // TODO: Validate username here.
        var user = new User { Id = Guid.NewGuid().ToString(), Username = request.Username };
        if (!RegisteredUsers.TryAdd(user.Id, user))
            throw new RpcException(new Status(StatusCode.ResourceExhausted, "Could not register user"));
        return Task.FromResult(new RegisterResponse { User = user });
    }

    public override Task<Empty> Login(LoginRequest request, ServerCallContext context)
    {
        if (!RegisteredUsers.ContainsKey(request.User.Id))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not registered"));
        
        var userId = request.User.Id;
        if (LoggedUsers.TryAdd(userId, request.User))
            return EmptyTaskResponse;

        if (LoggedUsers.ContainsKey(userId))
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Already logged in"));

        throw new RpcException(new Status(StatusCode.Unknown, "Could not login"));
    }

    public override async Task Join(JoinRequest request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        ValidateUser(request.User);
        ValidateChannel(request.Chat);

        var channelId = request.Chat.Id;
        var subscription = Subscriptions[channelId];

        var userId = request.User.Id;
        subscription.TryAdd(userId, responseStream);

        var messageBuffer = MessageBuffers[channelId];
        messageBuffer.Post(new Message { User = Server, Text = $"{request.User.Username}#{userId} has joined channel '{request.Chat.Name}'." });

        while (subscription.ContainsKey(userId))
        {
            var message = await messageBuffer.ReceiveAsync();
            await Task.WhenAll(subscription.Values.Select(s => s.WriteAsync(message)));
        }
    }

    public override Task<Empty> Send(Message request, ServerCallContext context)
    {
        ValidateUser(request.User);
        ValidateChannel(request.Chat);
        var messageBuffer = MessageBuffers[request.Chat.Id];
        messageBuffer.Post(request);
        return EmptyTaskResponse;
    }

    public override Task<Empty> Leave(LeaveRequest request, ServerCallContext context)
    {
        ValidateUser(request.User);
        ValidateChannel(request.Chat);
        var chatId = request.Chat.Id;
        var subscription = Subscriptions[chatId];
        var userId = request.User.Id;
        subscription.TryRemove(userId, out _);
        LoggedUsers.TryRemove(userId, out _);
        MessageBuffers[chatId].Post(new Message { User = Server, Text = $"{request.User.Username}#{userId} has left channel '{request.Chat.Name}'." });
        return EmptyTaskResponse;
    }

    public override Task<GetChatResponse> GetChat(GetChatRequest request, ServerCallContext context)
    {
        var response = new GetChatResponse();
        switch (request.FilterCase)
        {
            case GetChatRequest.FilterOneofCase.ChatId:
                response.Chats.Add(Chats[request.ChatId]);
                break;
            case GetChatRequest.FilterOneofCase.ChatName:
                response.Chats.AddRange(Chats.Values.Where(chat => chat.Name == request.ChatName));
                break;
            default:
                response.Chats.AddRange(Chats.Values);
                break;
        }

        if (response.Chats.Count == 0)
            throw new RpcException(new Status(StatusCode.NotFound, "No chats"));

        return Task.FromResult(response);
    }

    public override Task<PostChatResponse> PostChat(PostChatRequest request, ServerCallContext context)
    {
        var chat = new Chat { Id = Guid.NewGuid().ToString(), Name = request.Name };
        if (!Chats.TryAdd(chat.Id, chat))
            throw new RpcException(new Status(StatusCode.Unknown, "Could not add chat"));
        
        Subscriptions.TryAdd(chat.Id, new ConcurrentDictionary<string, IServerStreamWriter<Message>>());
        MessageBuffers.TryAdd(chat.Id, new BufferBlock<Message>(new DataflowBlockOptions { EnsureOrdered = true }));
        return Task.FromResult(new PostChatResponse { Chat = chat });
    }

    public override Task<Empty> DeleteChat(DeleteChatRequest request, ServerCallContext context)
    {
        var chatId = request.Chat.Id;
        if (!Chats.TryRemove(chatId, out _))
            throw new RpcException(new Status(StatusCode.NotFound, $"Chat '{request.Chat.Id}' does not exist"));
        
        MessageBuffers[chatId].Complete();
        MessageBuffers.TryRemove(chatId, out _);
        Subscriptions[chatId].Clear();
        Subscriptions.TryRemove(chatId, out _);
        
        return EmptyTaskResponse;
    }
}
