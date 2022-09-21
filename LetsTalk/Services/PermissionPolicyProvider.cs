using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace LetsTalk.Services;

internal sealed class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IAuthorizationPolicyProvider _fallbackProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        // There can only be one policy provider in ASP.NET Core.
        // We only handle permissions related policies, for the rest
        // we will use the default provider.
        _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _fallbackProvider.GetFallbackPolicyAsync();
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if(policyName.StartsWith("Permissions", StringComparison.InvariantCultureIgnoreCase))
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement { Permission = policyName })
                .Build();
            return Task.FromResult<AuthorizationPolicy?>(policy);
        }
        return _fallbackProvider.GetPolicyAsync(policyName);
    }
}
