using LetsTalk.Services;

namespace LetsTalk;
internal sealed class AccessTokenProvider : IAccessTokenProvider
{
    public string? AccessToken { get; set; }

    public Task<string?> GetAccessTokenAsync() => Task.FromResult(AccessToken);
}
