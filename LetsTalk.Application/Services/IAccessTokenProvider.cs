namespace LetsTalk.Services;
public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}
