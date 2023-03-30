using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace LetsTalk.Services;


internal sealed class UserProfileService : IProfileService
{
    private readonly IUserStore _userStore;
    private readonly ILogger _logger;

    public UserProfileService(IUserStore userStore, ILogger<UserProfileService> logger)
    {
        _userStore = userStore;
        _logger = logger;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        context.LogProfileRequest(_logger);

        if (context.RequestedClaimTypes.Any())
        {
            var user = await _userStore.FindBySubjectIdAsync(context.Subject.GetSubjectId());
            if (user is not null)
            {
                var userClaims = user.GetClaims();
                context.AddRequestedClaims(userClaims);
            }
        }

        context.LogIssuedClaims(_logger);
    }

    /// <summary>
    /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
    /// (e.g. during token issuance or validation).
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    public async Task IsActiveAsync(IsActiveContext context)
    {
        _logger.LogDebug("IsActive called from: {caller}", context.Caller);

        var user = await _userStore.FindBySubjectIdAsync(context.Subject.GetSubjectId());
        context.IsActive = user?.IsActive is true;
    }
}
