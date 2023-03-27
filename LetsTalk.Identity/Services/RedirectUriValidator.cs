using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;

namespace LetsTalk.Services;

public sealed class RedirectUriValidator : StrictRedirectUriValidator
{
#if DEBUG
    public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
        return await base.IsRedirectUriValidAsync(requestedUri, client) || requestedUri.StartsWith("https://localhost");
    }

    public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
        return await base.IsPostLogoutRedirectUriValidAsync(requestedUri, client) || requestedUri.StartsWith("https://localhost");
    }
#endif
}
