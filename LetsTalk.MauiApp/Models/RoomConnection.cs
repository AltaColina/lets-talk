using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Rooms;
using LetsTalk.Messaging;
using System.Collections.ObjectModel;

namespace LetsTalk.Models;
public sealed partial class RoomConnection : ObservableObject, IRecipient<ContentMessage>
{
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private int _newMessageCount;

    [ObservableProperty]
    private bool _isRoomVisible;

    public RoomDto Room { get; }

    public ObservableCollection<ContentMessage> Messages { get; } = new();

    public RoomConnection(IMessenger messenger, RoomDto room)
    {
        _messenger = messenger;
        Room = room;
        _messenger.Register(this, room.Id);
    }

    partial void OnIsRoomVisibleChanged(bool value)
    {
        if (value)
            NewMessageCount = 0;
    }

    void IRecipient<ContentMessage>.Receive(ContentMessage message)
    {
        Messages.Add(message);
        if (!IsRoomVisible)
            NewMessageCount++;
    }
}
