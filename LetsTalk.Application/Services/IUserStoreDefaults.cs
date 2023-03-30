namespace LetsTalk.Services;
public interface IUserStoreDefaults
{
    Task<List<string>> GetDefaultRolesAsync();

    Task<List<string>> GetDefaultRoomsAsync();
}