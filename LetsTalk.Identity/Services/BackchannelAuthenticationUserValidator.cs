using Duende.IdentityServer.Validation;
using IdentityModel;
using LetsTalk.Users;
using System.Security.Claims;

namespace LetsTalk.Services;

internal sealed class BackchannelAuthenticationUserValidator : IBackchannelAuthenticationUserValidator
{
    private readonly IUserStore _userStore;

    public BackchannelAuthenticationUserValidator(IUserStore userStore)
    {
        _userStore = userStore;
    }

    public async Task<BackchannelAuthenticationUserValidationResult> ValidateRequestAsync(BackchannelAuthenticationUserValidatorContext userValidatorContext)
    {
        var result = new BackchannelAuthenticationUserValidationResult();

        var user = default(User);

        if (userValidatorContext.LoginHint is not null)
        {
            user = await _userStore.FindByUsernameAsync(userValidatorContext.LoginHint);
        }
        else if (userValidatorContext.IdTokenHintClaims is not null && userValidatorContext.IdTokenHintClaims.SingleOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value is string subjectId)
        {
            user = await _userStore.FindBySubjectIdAsync(subjectId);
        }

        if (user is not null && user.IsActive)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
            };
            var ci = new ClaimsIdentity(claims, "ciba");
            result.Subject = new ClaimsPrincipal(ci);
        }

        return result;
    }
}
