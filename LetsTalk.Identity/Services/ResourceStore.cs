using Ardalis.Specification;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using LetsTalk.Repositories;

namespace LetsTalk.Services;

internal sealed class ResourceStore : IResourceStore
{
    private readonly IIdentityResourceRepository _identityResourceRepository;
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IApiScopeRepository _apiScopeRepository;

    public ResourceStore(IIdentityResourceRepository identityResourceRepository, IApiResourceRepository apiResourceRepository, IApiScopeRepository apiScopeRepository)
    {
        _identityResourceRepository = identityResourceRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
    }

    public async Task<Resources> GetAllResourcesAsync()
    {
        var identities = await _identityResourceRepository.ListAsync();
        var resources = await _apiResourceRepository.ListAsync();
        var scopes = await _apiScopeRepository.ListAsync();
        return new Resources(identities, resources, scopes);
    }

    private sealed class ApiResourceByNameSpecification : Specification<ApiResource>
    {
        public ApiResourceByNameSpecification(IEnumerable<string> apiResourceNames)
        {
            Query.Where(r => apiResourceNames.Contains(r.Name));
        }
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        if (apiResourceNames == null)
            throw new ArgumentNullException(nameof(apiResourceNames));

        return await _apiResourceRepository.ListAsync(new ApiResourceByNameSpecification(apiResourceNames));
    }

    private sealed class ApiResourceByScopeNamesSpecification : Specification<ApiResource>
    {
        public ApiResourceByScopeNamesSpecification(IEnumerable<string> scopeNames)
        {
            Query.Where(r => r.Scopes.Any(s => scopeNames.Contains(s)));
        }
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));

        return await _apiResourceRepository.ListAsync(new ApiResourceByScopeNamesSpecification(scopeNames));
    }

    private sealed class ResourcesByScopeNameSpecification<T> : Specification<T> where T : Resource
    {
        public ResourcesByScopeNameSpecification(IEnumerable<string> scopeNames)
        {
            Query.Where(r => scopeNames.Contains(r.Name));
        }
    }

    public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));

        return await _identityResourceRepository.ListAsync(new ResourcesByScopeNameSpecification<IdentityResource>(scopeNames));
    }

    public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));

        return await _apiScopeRepository.ListAsync(new ResourcesByScopeNameSpecification<ApiScope>(scopeNames));
    }
}
