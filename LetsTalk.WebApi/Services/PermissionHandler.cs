using Ardalis.Specification;
using LetsTalk.Repositories;
using LetsTalk.Roles;
using LetsTalk.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace LetsTalk.Services;

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    [NotNull] public string? Permission { get; init; }
}

internal sealed class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IDistributedCache _distributedCache;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;

    public PermissionHandler(IDistributedCache distributedCache, IRoleRepository roleRepository, IUserRepository userRepository)
    {
        _distributedCache = distributedCache;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    private sealed class GetUserRolesSpecification : Specification<Role>
    {
        public GetUserRolesSpecification(User user)
        {
            Query.Where(r => user.Roles.Contains(r.Id));
        }
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.Name is string userId)
        {
            var cacheKey = $"roles_{userId}";
            var roles = default(List<Role>);
            var rolesJson = await _distributedCache.GetStringAsync(cacheKey);
            if (rolesJson is not null)
            {
                roles = JsonSerializer.Deserialize<List<Role>>(rolesJson)!;
            }
            else
            {
                var user = (await _userRepository.GetByIdAsync(userId))!;
                roles = await _roleRepository.ListAsync(new GetUserRolesSpecification(user));
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(roles));
            }

            if (roles.Any(role => role.Permissions.Contains(requirement.Permission)))
                context.Succeed(requirement);
        }
    }
}
