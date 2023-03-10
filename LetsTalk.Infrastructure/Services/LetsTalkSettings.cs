using LetsTalk.Security;

namespace LetsTalk.Services;
internal sealed class LetsTalkSettings : ILetsTalkSettings
{
    public Authentication? Authentication { get; set; }
    public Task<string?> ProvideToken() => Task.FromResult(Authentication?.AccessToken);
}
