using LetsTalk.Users;

namespace LetsTalk.Interfaces;

public interface IHubConnectionManager
{
    void AddMapping(string connectionId, User user);
    IReadOnlyCollection<string> GetConnectionIds();
    IReadOnlyCollection<string> GetUserIds();
    string GetConnectionId(string userId);
    string GetUserId(string connectionId);
    void RemoveMapping(string connectionId);
}
