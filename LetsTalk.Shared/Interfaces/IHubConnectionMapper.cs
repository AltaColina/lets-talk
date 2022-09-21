namespace LetsTalk.Interfaces;

public interface IHubConnectionMapper
{
    void AddMapping(string connectionId, string userId);
    IEnumerable<string> GetConnectionIds();
    IEnumerable<string> GetUserIds();
    string GetConnectionId(string userId);
    string GetUserId(string connectionId);
    void RemoveMapping(string connectionId);
}
