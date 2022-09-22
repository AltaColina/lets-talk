using LetsTalk.Interfaces;
using LetsTalk.Models;

namespace LetsTalk.Services;

internal sealed class HubConnectionManager : IHubConnectionManager
{
    private readonly object _mutex = new();
    private readonly Dictionary<string, string> _connectionIdToUserId = new();
    private readonly Dictionary<string, string> _userIdToConnectionId = new();

    public void AddMapping(string connectionId, User user)
    {
        lock (_mutex)
        {
            _connectionIdToUserId[connectionId] = user.Id;
            _userIdToConnectionId[user.Id] = connectionId;
        }
    }

    public void RemoveMapping(string connectionId)
    {
        lock (_mutex)
        {
            if (_connectionIdToUserId.Remove(connectionId, out var userId))
                _userIdToConnectionId.Remove(userId);
        }
    }

    public IReadOnlyCollection<string> GetConnectionIds() => _connectionIdToUserId.Keys;

    public IReadOnlyCollection<string> GetUserIds() => _userIdToConnectionId.Keys;

    public string GetUserId(string connectionId) => _connectionIdToUserId[connectionId];

    public string GetConnectionId(string userId) => _userIdToConnectionId[userId];
}
