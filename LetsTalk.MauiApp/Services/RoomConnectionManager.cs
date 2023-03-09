using CommunityToolkit.Mvvm.Messaging;
using LetsTalk.Models;
using LetsTalk.Rooms;
using System.Collections.ObjectModel;

namespace LetsTalk.Services;

public sealed class RoomConnectionManager
{
    private readonly IMessenger _messenger;

	public ObservableCollection<RoomConnection> Connections { get; private set; } = new();

    public RoomConnectionManager(IMessenger messenger)
    {
        _messenger = messenger;
    }

	public void Add(RoomDto room) => Connections.Add(new(_messenger, room));

    public void Remove(RoomDto room) => Connections.Remove(new(_messenger, room));

    public void Reset(IEnumerable<RoomDto> rooms) => Connections = new(rooms.Select(room => new RoomConnection(_messenger, room)));
}
