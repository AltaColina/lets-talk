using AutoMapper;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;
using LetsTalk;
using LetsTalk.Repositories;
using LetsTalk.Roles;
using LetsTalk.Rooms;
using LetsTalk.Security;
using LetsTalk.Services;
using LetsTalk.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCryptography(this IServiceCollection services, IConfiguration configuration)
    {
        var hashType = typeof(MD5).Assembly.GetType($"{typeof(MD5).Namespace}.{configuration.GetRequiredSection("HashAlgorithm").Value!}", throwOnError: true)!;
        if (hashType.GetMethod(nameof(MD5.Create), Array.Empty<Type>())!.Invoke(null, null) is not HashAlgorithm instance)
            throw new InvalidOperationException($"Could not create hash algorithm '{configuration.GetRequiredSection("HashAlgorithm").Value}'");
        services.AddSingleton<HashAlgorithm>(instance);
        services.AddSingleton<IPasswordHandler, PasswordHandler>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Caching.
        services.AddStackExchangeRedisCache(opts => opts.Configuration = configuration.GetConnectionString("RedisCache"));

        // Hubs.
        services.AddSingleton<IHubConnectionManager, HubConnectionManager>();

        // Database.
        MongoBsonMapper.RegisterClassMaps();
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDb")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("letstalk"));

        // Repositories.
        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddSingleton<IRoleRepository, RoleRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IApiResourceRepository, ApiResourceRepository>();
        services.AddSingleton<IApiScopeRepository, ApiScopeRepository>();
        services.AddSingleton<IIdentityResourceRepository, IdentityResourceRepository>();
        services.AddSingleton<IClientRepository, ClientRepository>();

        return services;
    }

    public static async Task InsertResourceDataAsync<THost>(this THost host, IConfiguration configuration, bool overwrite = false) where THost : IHost
    {
        var database = host.Services.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
        var apiResourceRepository = host.Services.GetRequiredService<IApiResourceRepository>();
        var apiScopeRepository = host.Services.GetRequiredService<IApiScopeRepository>();
        var identityResourceRepository = host.Services.GetRequiredService<IIdentityResourceRepository>();

        if (overwrite || !await apiResourceRepository.AnyAsync() || !await apiScopeRepository.AnyAsync() || !await identityResourceRepository.AnyAsync())
        {
            await database.ResetCollectionAsync<ApiResource>(apiResourceRepository.CollectionName);
            await database.ResetCollectionAsync<ApiScope>(apiScopeRepository.CollectionName);
            await database.ResetCollectionAsync<IdentityResource>(identityResourceRepository.CollectionName);

            var apiResources = new List<ApiResource>(); //configuration.GetRequiredSection("ApiResources").Get<List<ApiResource>>() ?? throw ExceptionFor<IConfiguration>.Invalid("ApiResources");
            apiResources.ValidateClaims();

            var apiScopes = configuration.GetRequiredSection("ApiScopes").Get<List<ApiScope>>() ?? throw ExceptionFor<IConfiguration>.Invalid("ApiScopes");
            apiScopes.ValidateClaims();

            var identityResources = configuration.GetRequiredSection("IdentityResources").Get<List<IdentityResource>>() ?? throw ExceptionFor<IConfiguration>.Invalid("IdentityResources");
            identityResources.EnforceValidStandardScopes();
            identityResources.ValidateClaims();

            if (apiResources.Count > 0)
                await apiResourceRepository.AddRangeAsync(apiResources);
            await apiScopeRepository.AddRangeAsync(apiScopes);
            await identityResourceRepository.AddRangeAsync(identityResources);
        }

        var clientRepository = host.Services.GetRequiredService<IClientRepository>();
        if (overwrite || !await clientRepository.AnyAsync())
        {
            await database.ResetCollectionAsync<Client>(clientRepository.CollectionName);

            var clients = configuration.GetRequiredSection("Clients").Get<List<Client>>() ?? throw ExceptionFor<IConfiguration>.Invalid("Clients");
            clients.ValidateGrantTypes();
            var validScopes = Enumerable.Empty<Resource>()
                .Concat(await apiResourceRepository.ListAsync())
                .Concat(await apiScopeRepository.ListAsync())
                .Concat(await identityResourceRepository.ListAsync())
                .Select(r => r.Name);
            clients.ValidateScopes(validScopes);

            await clientRepository.AddRangeAsync(clients);
        }
    }

    public static async Task InsertUserDataAsync<THost>(this THost host, IConfiguration configuration, bool overwrite = false) where THost : IHost
    {
        var database = host.Services.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
        var roleRepository = host.Services.GetRequiredService<IRoleRepository>();
        var userRepository = host.Services.GetRequiredService<IUserRepository>();
        var roomRepository = host.Services.GetRequiredService<IRoomRepository>();

        if (overwrite || !await roleRepository.AnyAsync() || !await userRepository.AnyAsync() || !await roomRepository.AnyAsync())
        {
            await database.ResetCollectionAsync<Role>(roleRepository.CollectionName, e => e.Name);
            await database.ResetCollectionAsync<User>(userRepository.CollectionName, e => e.Name);
            await database.ResetCollectionAsync<Room>(roomRepository.CollectionName, e => e.Name);

            var roles = configuration.GetRequiredSection("Roles").Get<List<Role>>() ?? throw ExceptionFor<IConfiguration>.Invalid("Roles");
            if (roles.SelectMany(r => r.Permissions).FirstOrDefault(p => !Permissions.IsValid(p)) is string permission)
                throw ExceptionFor<Role>.Invalid("permission", permission);

            var rooms = configuration.GetRequiredSection("Rooms").Get<List<Room>>() ?? throw ExceptionFor<IConfiguration>.Invalid("Rooms");

            var users = configuration.GetRequiredSection("Users").Get<List<User>>() ?? throw ExceptionFor<IConfiguration>.Invalid("Users");

            var userConfigs = configuration.GetRequiredSection("UserConfigurations").Get<Dictionary<string, UserConfiguration>>() ?? throw ExceptionFor<IConfiguration>.Invalid("UserConfigurations");
            foreach (var user in users)
            {
                if (!userConfigs.TryGetValue(user.Name, out var config))
                    throw ExceptionFor<UserConfiguration>.NotFound("User", user.Name);

                var userRoles = config.Roles.Select(name => roles.Single(r => r.Name == name)).ToList();
                user.Roles.UnionWith(userRoles.Select(role => role.Id));

                var userRooms = config.Rooms.Select(name => rooms.Single(r => r.Name == name)).ToList();
                user.Rooms.UnionWith(userRooms.Select(room => room.Id));
                userRooms.ForEach(room => room.Users.Add(user.Id));
            }

            await roleRepository.AddRangeAsync(roles);
            await roomRepository.AddRangeAsync(rooms);
            await userRepository.AddRangeAsync(users);
        }
    }
}

file static class DependencyInjectionExtensions
{
    private static readonly IReadOnlySet<string> ValidClaims = typeof(JwtClaimTypes)
        .GetFields(BindingFlags.Static | BindingFlags.Public).Select(p => (string)p.GetValue(null)!)
        .ToHashSet();

    private static readonly IReadOnlySet<string> ValidGrantTypes = typeof(GrantTypes)
        .GetProperties(BindingFlags.Static | BindingFlags.Public)
        .SelectMany(p => (ICollection<string>)p.GetValue(null)!)
        .ToHashSet();

    private static readonly IReadOnlyDictionary<string, Type> StandardIdentityResourceTypes = typeof(IdentityResources)
        .GetNestedTypes()
        .ToDictionary(t => t.Name);

    private static readonly IReadOnlyDictionary<string, Func<IdentityResource>> StandardScopesToIdentityResourcesMap = typeof(IdentityServerConstants.StandardScopes)
        .GetFields(BindingFlags.Static | BindingFlags.Public)
        .Where(f => StandardIdentityResourceTypes.ContainsKey(f.Name))
        .ToDictionary(
            f => (string)f.GetValue(null)!,
            f => GetActivator(f.Name));

    private static Func<IdentityResource> GetActivator(string name)
    {
        return () =>
        {
            var resource = Activator.CreateInstance(StandardIdentityResourceTypes[name]);
            return resource is null
                ? throw new InvalidOperationException($"Invalid IdentityResource '{name}'")
                : IdentityResourceMapping.Mapper.Map<IdentityResource>(resource);
        };
    }

    public static Task CreateUniqueTextIndex<T>(this IMongoIndexManager<T> indices, Expression<Func<T, object>> keySelector)
    {
        return indices.CreateOneAsync(new CreateIndexModel<T>(Builders<T>.IndexKeys.Text(keySelector), new CreateIndexOptions { Unique = true }));
    }

    public static async Task ResetCollectionAsync<T>(this IMongoDatabase database, string collectionName, params Expression<Func<T, object>>[] indices)
    {
        var collection = database.GetCollection<T>(collectionName);
        await collection.DeleteManyAsync(FilterDefinition<T>.Empty);
        await collection.Indexes.DropAllAsync();
        foreach (var index in indices)
            await collection.Indexes.CreateUniqueTextIndex(index);
    }

    public static void ValidateClaims<T>(this IEnumerable<T> resources) where T : Resource
    {
        if (resources.FirstOrDefault(r => r.UserClaims.Any(c => !ValidClaims.Contains(c))) is T invalidResource)
            throw ExceptionFor<T>.Invalid("claims", invalidResource.UserClaims.Where(c => !ValidClaims.Contains(c)));
    }

    public static void EnforceValidStandardScopes(this List<IdentityResource> resources)
    {
        for (int i = 0; i < resources.Count; ++i)
        {
            if (StandardScopesToIdentityResourcesMap.TryGetValue(resources[i].Name, out var createStandardResource))
                resources[i] = createStandardResource.Invoke();
        }
    }

    public static void ValidateGrantTypes(this IEnumerable<Client> clients)
    {
        if (clients.FirstOrDefault(r => r.AllowedGrantTypes.Any(c => !ValidGrantTypes.Contains(c))) is Client invalidClient)
            throw ExceptionFor<Client>.Invalid("grant types", invalidClient.AllowedGrantTypes.Where(c => !ValidGrantTypes.Contains(c)));
    }

    public static void ValidateScopes(this IEnumerable<Client> clients, IEnumerable<string> validScopes)
    {
        if (clients.FirstOrDefault(r => r.AllowedScopes.Any(c => !validScopes.Contains(c))) is Client invalidClient)
            throw ExceptionFor<Client>.Invalid("api scopes", invalidClient.AllowedScopes.Where(c => !validScopes.Contains(c)));
    }

    private sealed class IdentityResourceMapping : Profile
    {
        public static Mapper Mapper { get; } = new(new MapperConfiguration(e => e.AddProfile(new IdentityResourceMapping())));

        private IdentityResourceMapping()
        {
            foreach (var type in StandardIdentityResourceTypes.Values)
                CreateMap(type, typeof(IdentityResource));
        }
    }
}

file sealed class UserConfiguration
{
    public required List<string> Roles { get; init; }
    public required List<string> Rooms { get; init; }
}