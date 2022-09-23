using Ardalis.Specification;
using LetsTalk.Interfaces;
using LetsTalk.Models;
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
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<User> _userRepository;

    public PermissionHandler(IDistributedCache distributedCache, IRepository<Role> roleRepository, IRepository<User> userRepository)
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
        if (context.User.Identity?.Name is string username)
        {
            var cacheKey = $"roles_{username}";
            var roles = default(List<Role>);
            var rolesJson = await _distributedCache.GetStringAsync(cacheKey);
            if (rolesJson is not null)
            {
                roles = JsonSerializer.Deserialize<List<Role>>(rolesJson)!;
            }
            else
            {
                var user = (await _userRepository.GetByIdAsync(username))!;
                roles = await _roleRepository.ListAsync(new GetUserRolesSpecification(user));
                await _distributedCache.SetStringAsync(cacheKey, JsonSerializer.Serialize(roles));
            }

            if (roles.Any(role => role.Permissions.Contains(requirement.Permission)))
                context.Succeed(requirement);
        }
    }
}
