using LetsTalk.Interfaces;
using LetsTalk.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace LetsTalk.Services;

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    [NotNull] public string? Permission { get; init; }
}

internal sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;

    public PermissionHandler(IRepository<Role> roleRepository, IRepository<User> userRepository)
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
