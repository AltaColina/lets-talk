using LetsTalk.Interfaces;

namespace LetsTalk.Services;

internal sealed class HubConnectionMapper : IHubConnectionMapper
{
    private readonly object _mutex = new();
    private readonly Dictionary<string, string> _connectionIdToUserId = new();
    private readonly Dictionary<string, string> _userIdToConnectionId = new();

    public void AddMapping(string connectionId, string userId)
    {
        lock (_mutex)
        {
            _connectionIdToUserId[connectionId] = userId;
            _userIdToConnectionId[userId] = connectionId;
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

    public IEnumerable<string> GetConnectionIds() => _connectionIdToUserId.Keys;

    public IEnumerable<string> GetUserIds() => _userIdToConnectionId.Keys;

    public string GetUserId(string connectionId) => _connectionIdToUserId[connectionId];

    public string GetConnectionId(string userId) => _userIdToConnectionId[userId];
}
