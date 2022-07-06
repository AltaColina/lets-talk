using LetsTalk.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Services;

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    [NotNull] public string? Permission { get; init; }
}

internal sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public PermissionHandler(IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.Name is not null)
        {
            var user = (await _userRepository.GetAsync(context.User.Identity.Name))!;
            var roles = (await _roleRepository.GetAllAsync()).Where(role => user.Roles.Contains(role.Id));
            if (roles.Any(role => role.Permissions.Contains(requirement.Permission)))
                context.Succeed(requirement);
        }
    }
}
