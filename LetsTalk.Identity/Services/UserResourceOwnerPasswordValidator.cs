using Duende.IdentityServer.Validation;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace LetsTalk.Services;

internal sealed class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IUserStore _userStore;
    private readonly ISystemClock _systemClock;

    public UserResourceOwnerPasswordValidator(IUserStore userStore, ISystemClock systemClock)
    {
        _userStore = userStore;
        _systemClock = systemClock;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await _userStore.FindByUsernameAsync(context.UserName);
        if (user is not null)
            context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password, _systemClock.UtcNow.UtcDateTime, user.GetClaims());
    }
}
