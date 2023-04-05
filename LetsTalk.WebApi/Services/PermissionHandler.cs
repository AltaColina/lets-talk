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

    public PermissionHandler(IDistributedCache distributedCache, IRoleRepository roleRepository)
    {
        _distributedCache = distributedCache;
        _roleRepository = roleRepository;
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
        var roleIds = context.User!.GetRoles();
        if (roleIds.Any())
        {
            const string key = "permissions";
            var permissions = await _distributedCache.GetFromJsonAsync<Dictionary<string, HashSet<string>>>(key);
            if (permissions is null)
            {
                permissions = (await _roleRepository.ListAsync()).ToDictionary(r => r.Id, r => r.Permissions);
                await _distributedCache.SetFromJsonAsync(key, permissions);
            }
            if (roleIds.Any(id => permissions.TryGetValue(id, out var list) && list.Contains(requirement.Permission)))
                context.Succeed(requirement);
        }
    }
}

file static class DistributedCacheExtensions
{
    private static readonly DistributedCacheEntryOptions DefaultEntryOptions = new();

    public static async Task<T?> GetFromJsonAsync<T>(this IDistributedCache cache, string key)
    {
        var json = await cache.GetAsync(key);
        return json is not null ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public static async Task SetFromJsonAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions? options = null)
    {
        var json = JsonSerializer.SerializeToUtf8Bytes(value);
        await cache.SetAsync(key, json, options ?? DefaultEntryOptions);
    }
}

