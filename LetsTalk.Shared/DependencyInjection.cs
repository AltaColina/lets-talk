using Ardalis.Specification;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentValidation;
using LetsTalk.App.Services;
using LetsTalk.Behaviors;
using LetsTalk.Chats;
using LetsTalk.Interfaces;
using LetsTalk.Profiles;
using LetsTalk.Roles;
using LetsTalk.Security;
using LetsTalk.Services;
using LetsTalk.Users;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IConfigurationBuilder AddContainersConfiguration(this IConfigurationBuilder configuration, string host, params string[] containerNames)
    {
        var dockerClient = new DockerClientConfiguration().CreateClient();
        var containers = Task.Run(async () => await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true })).Result;
        var connectionStrings = new Dictionary<string, string?>();
        foreach (var container in containers)
        {
            if (container.Names.SingleOrDefault(name => containerNames.Contains(name)) is string containerName)
            {
                var port = container.Ports.Single(p => p.PrivatePort == 443); // HTTPS
                connectionStrings[$"ConnectionStrings:{containerName.Replace("/", "")}"] = $"https://{host}:{port.PublicPort}";
            }
        }
        configuration.AddInMemoryCollection(connectionStrings);
        //configuration.AddInMemoryCollection(new Dictionary<string, string>
        //{
        //    ["ConnectionStrings:LetsTalk"] = $"http://{host}:64411"
        //});
        return configuration;
    }

    public static IServiceCollection AddCryptography(this IServiceCollection services, IConfiguration configuration)
    {
        var hashType = typeof(MD5).Assembly.GetType($"{typeof(MD5).Namespace}.{configuration.GetRequiredSection("HashAlgorithm").Value!}", throwOnError: true)!;
        if (hashType.GetMethod(nameof(MD5.Create), Array.Empty<Type>())!.Invoke(null, null) is not HashAlgorithm instance)
            throw new InvalidOperationException($"Could not create hash algorithm '{configuration.GetRequiredSection("HashAlgorithm").Value}'");
        services.AddSingleton<SecurityKey>(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSection("SecurityKey").Value!)));
        services.AddSingleton<HashAlgorithm>(instance);
        services.AddSingleton<IPasswordHandler, PasswordHandler>();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var currentAsm = typeof(DependencyInjection).Assembly;
        var callingAsm = Assembly.GetCallingAssembly();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddAutoMapper(config => config.AddProfile(new MappingProfile(currentAsm, callingAsm)));
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblies(currentAsm, callingAsm));
        services.AddValidatorsFromAssembly(currentAsm);
        services.AddValidatorsFromAssembly(callingAsm);
        return services;
    }
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(opts => opts.Configuration = configuration.GetConnectionString("RedisCache"));
        services.AddSingleton<IHubConnectionManager, HubConnectionManager>();
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("letstalk"));
        services.AddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));

        return services;
    }

    public static IServiceCollection AddLetsTalkSettings(this IServiceCollection services)
    {
        services.AddSingleton<ILetsTalkSettings, LetsTalkSettings>();
        return services;
    }

    public static IServiceCollection AddLetsTalkHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(opts => opts.BaseAddress = new(configuration.GetConnectionString("LetsTalk")!));
        return services;
    }

    public static IServiceCollection AddLetsTalkHubClient(this IServiceCollection services)
    {
        services.AddSingleton<ILetsTalkHubClient, LetsTalkHubClient>();
        return services;
    }

    public static async Task LoadDatabaseData<THost>(this THost host, IConfiguration configuration, bool overwrite = false) where THost : IHost
    {
        var database = host.Services.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
        var roleRepository = host.Services.GetRequiredService<IRepository<Role>>();
        if (overwrite || !await roleRepository.AnyAsync())
        {
            database.GetCollection<Role>(roleRepository.CollectionName).DeleteMany(FilterDefinition<Role>.Empty);
            var roles = configuration.GetRequiredSection("Roles").Get<List<Role>>();
            if (roles is null)
                throw new InvalidOperationException("No default roles configured");
            var permissions = Permissions.All().ToList();
            foreach (var role in roles)
                if (role.Permissions.FirstOrDefault(p => !permissions.Contains(p)) is string permission)
                    throw new InvalidOperationException($"Permission '{permission}' is not valid");
            await roleRepository.AddRangeAsync(roles);
        }

        var userRepository = host.Services.GetRequiredService<IRepository<User>>();
        if (overwrite || !await userRepository.AnyAsync())
        {
            database.GetCollection<User>(userRepository.CollectionName).DeleteMany(FilterDefinition<User>.Empty);
            var users = configuration.GetRequiredSection("Users").Get<List<User>>();
            if (users is null)
                throw new InvalidOperationException("No default users configured");
            await Task.WhenAll(users.Select(async user =>
            {
                var roles = await roleRepository.ListAsync(new GenericSpec<Role>(q => q.Where(r => user.Roles.Contains(r.Id))));
                if (roles.Count != user.Roles.Count)
                    throw new InvalidOperationException($"Roles '{String.Join(", ", user.Roles.Except(roles.Select(r => r.Id)))}' are not valid");
            }));
            await userRepository.AddRangeAsync(users);
        }

        var chatRepository = host.Services.GetRequiredService<IRepository<Chat>>();
        if (overwrite || !await chatRepository.AnyAsync())
        {
            database.GetCollection<Chat>(chatRepository.CollectionName).DeleteMany(FilterDefinition<Chat>.Empty);
            var chats = configuration.GetRequiredSection("Chats").Get<List<Chat>>();
            if (chats is null)
                throw new InvalidOperationException("No default chats configured");
            await Task.WhenAll(chats.Select(async chat =>
            {
                var users = await userRepository.ListAsync(new GenericSpec<User>(q => q.Where(u => chat.Users.Contains(u.Id))));
                if (users.Count != chat.Users.Count)
                    throw new InvalidOperationException($"Users '{String.Join(", ", chat.Users.Except(users.Select(r => r.Id)))}' are not valid");
            }));
            await chatRepository.AddRangeAsync(chats);
        }
    }
}

file sealed class GenericSpec<T> : Specification<T>
{
    public GenericSpec(Action<ISpecificationBuilder<T>> configure)
    {
        configure.Invoke(Query);
    }
}