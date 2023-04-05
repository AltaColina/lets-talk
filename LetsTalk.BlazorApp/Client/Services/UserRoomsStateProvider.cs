using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Messaging;
using LetsTalk.Rooms;

namespace LetsTalk.Services;

public sealed class UserRoomsStateProvider : IRecipient<ContentMessage>
{
    private readonly IMessenger _messenger;

    private IReadOnlyDictionary<string, RoomDto> _roomsById = new Dictionary<string, RoomDto>();
    public IReadOnlyDictionary<string, RoomDto> RoomsById
    {
        get => _roomsById;
        set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));
            if (_roomsById != value)
            {
                _roomsById = value;
                StateChanged?.Invoke();
            }
        }
    }

    private readonly Dictionary<string, IReadOnlyList<ContentMessage>> _messagesByRoomId = new();
    public IEnumerable<ContentMessage> GetRoomMessages(string roomId)
    {
        return _messagesByRoomId.GetValueOrDefault(roomId) ?? Enumerable.Empty<ContentMessage>();
    }

    public event Action? StateChanged;

    public UserRoomsStateProvider(IMessenger messenger)
    {
        _messenger = messenger;
        _messenger.Register(this);
    }

    void IRecipient<ContentMessage>.Receive(ContentMessage message)
    {
        if (!_messagesByRoomId.TryGetValue(message.RoomId, out var list))
            _messagesByRoomId[message.RoomId] = list = new List<ContentMessage>();

        ((List<ContentMessage>)list).Add(message);
    }
}