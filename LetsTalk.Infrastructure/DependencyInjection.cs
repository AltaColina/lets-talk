using LetsTalk;
using LetsTalk.Exceptions;
using LetsTalk.Repositories;
using LetsTalk.Roles;
using LetsTalk.Rooms;
using LetsTalk.Security;
using LetsTalk.Services;
using LetsTalk.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCryptography(this IServiceCollection services, IConfiguration configuration)
    {
        var hashType = typeof(MD5).Assembly.GetType($"{typeof(MD5).Namespace}.{configuration.GetRequiredSection("HashAlgorithm").Value!}", throwOnError: true)!;
        if (hashType.GetMethod(nameof(MD5.Create), Array.Empty<Type>())!.Invoke(null, null) is not HashAlgorithm instance)
            throw new InvalidOperationException($"Could not create hash algorithm '{configuration.GetRequiredSection("HashAlgorithm").Value}'");
        services.AddSingleton<SecurityKey>(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSection("SecurityKey").Value!)));
        services.AddSingleton<HashAlgorithm>(instance);
        services.AddSingleton<IPasswordHandler, PasswordHandler>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();
        services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(opts => opts.Configuration = configuration.GetConnectionString("RedisCache"));
        services.AddSingleton<IHubConnectionManager, HubConnectionManager>();
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDb")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("letstalk"));
        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddSingleton<IRoleRepository, RoleRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();

        return services;
    }

    public static IServiceCollection AddLetsTalk(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILetsTalkSettings, LetsTalkSettings>();
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(opts => opts.BaseAddress = new(configuration.GetConnectionString("LetsTalk.WebApi")!));
        services.AddSingleton<ILetsTalkHubClient, LetsTalkHubClient>();
        return services;
    }

    public static async Task LoadDatabaseData<THost>(this THost host, IConfiguration configuration, bool overwrite = false) where THost : IHost
    {
        var database = host.Services.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
        var roleRepository = host.Services.GetRequiredService<IRoleRepository>();
        var userRepository = host.Services.GetRequiredService<IUserRepository>();
        var roomRepository = host.Services.GetRequiredService<IRoomRepository>();

        if (overwrite || !await roleRepository.AnyAsync() || !await userRepository.AnyAsync() || !await roomRepository.AnyAsync())
        {
            var roleCollection = database.GetCollection<Role>(roleRepository.CollectionName);
            var userCollection = database.GetCollection<User>(userRepository.CollectionName);
            var roomCollection = database.GetCollection<Room>(roomRepository.CollectionName);

            await roleCollection.ResetCollection();
            await roomCollection.ResetCollection();
            await userCollection.ResetCollection();

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

file static class MongoIndexExtensions
{
    public static Task CreateUniqueTextIndex<T>(this IMongoIndexManager<T> indices, Expression<Func<T, object>> keySelector)
    {
        return indices.CreateOneAsync(new CreateIndexModel<T>(Builders<T>.IndexKeys.Text(keySelector), new CreateIndexOptions { Unique = true }));
    }

    public static async Task ResetCollection<T>(this IMongoCollection<T> collection) where T : Entity
    {
        await collection.DeleteManyAsync(FilterDefinition<T>.Empty);
        await collection.Indexes.DropAllAsync();
        await collection.Indexes.CreateUniqueTextIndex(room => room.Name);
    }
}

file sealed class UserConfiguration
{
    public required List<string> Roles { get; init; }
    public required List<string> Rooms { get; init; }
}