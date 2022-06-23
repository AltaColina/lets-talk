using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LetsTalk.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;

namespace LetsTalk.Services;

[Authorize]
public class LetsTalkService : LetsTalk.LetsTalkBase
{
    private static readonly Regex UsernameRegex = new("^[a-zA-Z][a-z0-9_-]{3,15}$");
    private static readonly Person Server = new() { Username = "Server" };
    private static readonly Task<Empty> EmptyTaskResponse = Task.FromResult(new Empty());
    private static readonly ConcurrentDictionary<string, Chat> Chats = new();
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<Message>>> Subscriptions = new();
    private static readonly ConcurrentDictionary<string, BufferBlock<Message>> MessageBuffers = new();
    private readonly IPasswordHandler _passwordHandler;
    private readonly IAuthenticationManager _authenticationManager;
    private readonly IUserRepository _userRepository;

    static LetsTalkService()
    {
        var chat = new Chat { Id = Guid.NewGuid().ToString(), Name = "General" };
        if (Chats.TryAdd(chat.Id, chat))
        {
            Subscriptions.TryAdd(chat.Id, new ConcurrentDictionary<string, IServerStreamWriter<Message>>());
            MessageBuffers.TryAdd(chat.Id, new BufferBlock<Message>(new DataflowBlockOptions { EnsureOrdered = true }));
        }
    }

    public LetsTalkService(IPasswordHandler passwordHandler, IAuthenticationManager authenticationManager, IUserRepository userRepository)
    {
        _passwordHandler = passwordHandler;
        _authenticationManager = authenticationManager;
        _userRepository = userRepository;
    }

    private static void ValidateChannel(Chat chat)
    {
        if (!Subscriptions.ContainsKey(chat.Id))
            throw new RpcException(new Status(StatusCode.NotFound, "Chat does not exist"));
    }

    [AllowAnonymous]
    public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        if (!UsernameRegex.IsMatch(request.Username))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid username"));

        if (_userRepository.Get(request.Username) is not null)
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Username already in use"));

        // TODO: Validate password?

        var creationDateTime = DateTime.UtcNow;
        var user = new User
        {
            Id = request.Username,
            Secret = _passwordHandler.Encrypt(request.Password, request.Username),
            CreationTime = creationDateTime,
            LastLoginTime = creationDateTime,
        };

        var token = _authenticationManager.GenerateToken(user);
        
        user.RefreshTokens.Add(new RefreshToken
        {
            Id = token.RefreshToken,
            ExpiresIn = DateTime.UnixEpoch.AddSeconds(token.RefreshTokenExpiresIn)
        });
        _userRepository.Insert(user);
        
        return Task.FromResult(new RegisterResponse
        {
            Person = new Person { Username = request.Username },
            Token = token });
    }

    [AllowAnonymous]
    public override Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        var token = _authenticationManager.Authenticate(request.Username, request.Password);
        if (token is null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Incorrect username or password"));
        
        // Update user.
        var user = _userRepository.Get(request.Username)!;
        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(new RefreshToken
        {
            Id = token.RefreshToken,
            ExpiresIn = DateTime.UnixEpoch.AddSeconds(token.RefreshTokenExpiresIn)
        });
        _userRepository.Update(user);

        return Task.FromResult(new LoginResponse
        {
            Person = new Person { Username = request.Username },
            Token = token
        });
    }

    public override Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
    {
        var userId = request.Person.Username;
        foreach (var (chatId, subscription) in Subscriptions)
            if (subscription.TryRemove(userId, out _))
                MessageBuffers[chatId].Post(new Message { Person = Server, Text = $"{userId} has joined channel '{Chats[chatId].Name}'." });
        return EmptyTaskResponse;
    }

    [AllowAnonymous]
    public override Task<RefreshResponse> Refresh(RefreshRequest request, ServerCallContext context)
    {
        var token = _authenticationManager.Refresh(request.Username, request.RefreshToken);
        if (token is null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Incorrect username or password"));

        // Update user.
        var user = _userRepository.Get(request.Username)!;
        user.LastLoginTime = DateTime.UtcNow;
        user.RefreshTokens.RemoveAll(token => token.Id == request.RefreshToken || token.ExpiresIn < user.LastLoginTime);
        user.RefreshTokens.Add(new RefreshToken
        {
            Id = token.RefreshToken,
            ExpiresIn = DateTime.UnixEpoch.AddSeconds(token.RefreshTokenExpiresIn)
        });
        _userRepository.Update(user);

        return Task.FromResult(new RefreshResponse
        {
            Person = new Person { Username = request.Username },
            Token = token
        });
    }

    public override async Task Join(JoinRequest request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        ValidateChannel(request.Chat);

        var chatId = request.Chat.Id;
        var subscription = Subscriptions[chatId];

        var userId = request.Person.Username;
        subscription.TryAdd(userId, responseStream);

        var messageBuffer = MessageBuffers[chatId];
        messageBuffer.Post(new Message { Person = Server, Text = $"{userId} has joined channel '{request.Chat.Name}'." });

        while (subscription.ContainsKey(userId))
        {
            var message = await messageBuffer.ReceiveAsync();
            await Task.WhenAll(subscription.Values.Select(s => s.WriteAsync(message)));
        }
    }

    public override Task<Empty> Leave(LeaveRequest request, ServerCallContext context)
    {
        ValidateChannel(request.Chat);
        var chatId = request.Chat.Id;
        var subscription = Subscriptions[chatId];
        var userId = request.Person.Username;
        subscription.TryRemove(userId, out _);
        MessageBuffers[chatId].Post(new Message { Person = Server, Text = $"{userId} has left channel '{request.Chat.Name}'." });
        return EmptyTaskResponse;
    }

    public override Task<Empty> Send(Message request, ServerCallContext context)
    {
        ValidateChannel(request.Chat);
        var messageBuffer = MessageBuffers[request.Chat.Id];
        messageBuffer.Post(request);
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
