using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Hubs.Commands;
using LetsTalk.Hubs.Queries;
using LetsTalk.Messaging;
using LetsTalk.Services;
using LetsTalk.Users.Queries;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetsTalk.Services;

internal sealed class LetsTalkHubClient : ILetsTalkHubClient
{
    private readonly Listener _listener;
    private readonly string _hubEndpoint;
    private readonly ILetsTalkSettings _settings;
    private readonly IMessenger _messenger;
    private HubConnection? _connection;

    public bool IsConnected { get; private set; }

    public LetsTalkHubClient(IConfiguration configuration, ILetsTalkSettings settings, IMessenger messenger)
    {
        _listener = new Listener(messenger);
        _hubEndpoint = $"{configuration.GetConnectionString("LetsTalk.WebApi")}/hubs/letstalk";
        _messenger = messenger;
        _settings = settings;
    }
    public async Task ConnectAsync()
    {
        if (_connection is not null)
            await DisconnectAsync();

        _connection = new HubConnectionBuilder()
            .WithUrl(_hubEndpoint, opts => opts.AccessTokenProvider = _settings.ProvideToken)
            .AddMessagePackProtocol()
            .Build();
        _listener.Attach(_connection);
        await _connection.StartAsync();
        IsConnected = true;
    }

    public async Task DisconnectAsync()
    {
        if (_connection is not null)
        {
            _listener.Detach();
            await _connection.StopAsync();
            IsConnected = false;
        }
    }

    public async Task<JoinRoomResponse> JoinRoomAsync(string roomId) =>
        await _connection!.InvokeAsync<JoinRoomResponse>(nameof(JoinRoomAsync), roomId);

    public async Task<LeaveRoomResponse> LeaveRoomAsync(string roomId) =>
        await _connection!.InvokeAsync<LeaveRoomResponse>(nameof(LeaveRoomAsync), roomId);

    public async Task SendContentMessageAsync(string roomId, string contentType, byte[] message) =>
        await _connection!.InvokeAsync(nameof(SendContentMessageAsync), roomId, contentType, message);

    public async Task<GetLoggedUsersResponse> GetLoggedUsersAsync() =>
        await _connection!.InvokeAsync<GetLoggedUsersResponse>(nameof(GetLoggedUsersAsync));

    public async Task<GetLoggedRoomUsersResponse> GetLoggedRoomUsersAsync(string roomId)
        => await _connection!.InvokeAsync<GetLoggedRoomUsersResponse>(nameof(GetLoggedRoomUsersAsync), roomId);

    public async Task<GetUserRoomsResponse> GetUserRoomsAsync() =>
        await _connection!.InvokeAsync<GetUserRoomsResponse>(nameof(GetUserRoomsAsync));

    public async Task<GetUserAvailableRoomsResponse> GetUserAvailableRoomsAsync() =>
        await _connection!.InvokeAsync<GetUserAvailableRoomsResponse>(nameof(GetUserAvailableRoomsAsync));

    private sealed class Ref<T>
    {
        public T? Value { get; set; }
    }

    private readonly struct Listener
    {
        private readonly IMessenger _messenger;
        private readonly Ref<HubConnection> _connectionRef;
        private readonly List<IDisposable> _handlers;
        public bool IsListening { get => _connectionRef.Value is not null; }

        public Listener(IMessenger messenger)
        {
            _messenger = messenger;
            _connectionRef = new Ref<HubConnection>();
            _handlers = new List<IDisposable>();
        }

        public void Attach(HubConnection connection)
        {
            if (IsListening)
                throw new InvalidOperationException("Already listening to a connection");
            _connectionRef.Value = connection;
            _handlers.AddRange(new IDisposable[]
            {
                connection.On<ConnectMessage>(Handle),
                connection.On<DisconnectMessage>(Handle),
                connection.On<JoinRoomMessage>(Handle),
                connection.On<LeaveRoomMessage>(Handle),
                connection.On<ContentMessage>(Handle)
            });
        }

        public void Detach()
        {
            if (!IsListening)
                throw new InvalidOperationException("Not listening to any connection");
            _handlers.ForEach(h => h.Dispose());
            _handlers.Clear();
            _connectionRef.Value = null;
        }

        private void Handle(ConnectMessage message)
        {
            _messenger.Send(message);
        }

        private void Handle(DisconnectMessage message)
        {
            _messenger.Send(message);
        }

        private void Handle(JoinRoomMessage message)
        {
            _messenger.Send(message);
            _messenger.Send(message, message.RoomId);
        }

        private void Handle(LeaveRoomMessage message)
        {
            _messenger.Send(message);
            _messenger.Send(message, message.RoomId);
        }

        private void Handle(ContentMessage message)
        {
            _messenger.Send(message);
            _messenger.Send(message, message.RoomId);
        }
    }

}

file static partial class HubConnectionExtensions
{
    public static IDisposable On<TMessage>(this HubConnection hubConnection, Action<TMessage> handler)
    {
        return hubConnection.On(typeof(TMessage).Name, handler);
    }
}