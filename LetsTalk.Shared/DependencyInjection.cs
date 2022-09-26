using Docker.DotNet;
using Docker.DotNet.Models;
using FluentValidation;
using LetsTalk.App.Services;
using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Profiles;
using LetsTalk.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IConfigurationBuilder AddContainersConfiguration(this IConfigurationBuilder configuration, string host, params string[] containerNames)
    {
        var dockerClient = new DockerClientConfiguration().CreateClient();
        var containers = Task.Run(async () => await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true })).Result;
        var connectionStrings = new Dictionary<string, string>();
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

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var currentAsm = typeof(DependencyInjection).Assembly;
        var callingAsm = Assembly.GetCallingAssembly();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddAutoMapper(config => config.AddProfile(new MappingProfile(currentAsm, callingAsm)));
        services.AddMediatR(currentAsm, callingAsm);
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

    public static IServiceCollection AddLetsTalkSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILetsTalkSettings, LetsTalkSettings>();
        return services;
    }

    public static IServiceCollection AddLetsTalkHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(opts => opts.BaseAddress = new(configuration.GetConnectionString("LetsTalk")));
        return services;
    }

    public static IServiceCollection AddLetsTalkHubClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILetsTalkHubClient, LetsTalkHubClient>();
        return services;
    }

    public static async Task LoadDatabaseData<THost>(this THost host, bool overwrite = false) where THost : IHost
    {
        var database = host.Services.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
        var roleRepository = host.Services.GetRequiredService<IRepository<Role>>();
        if (overwrite || !await roleRepository.AnyAsync())
        {
            database.GetCollection<Role>(roleRepository.CollectionName).DeleteMany(FilterDefinition<Role>.Empty);
            var allPermissions = GetStaticFieldNames(typeof(Permissions));
            await roleRepository.AddRangeAsync(new List<Role>
                {
                    new Role
                    {
                        Id = "user",
                        Name = "User",
                        Permissions = new HashSet<string>(allPermissions.Where(perm => perm.EndsWith("View", StringComparison.InvariantCultureIgnoreCase)))
                    },
                    new Role
                    {
                        Id = "admin",
                        Name = "Administrator",
                        Permissions = new HashSet<string>(allPermissions)
                    },
                });

            static IEnumerable<string> GetStaticFieldNames(Type type, string prefix = "")
            {
                prefix += $"{type.Name}.";

                var fields = type
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(field => (string)field.GetValue(null)!);

                foreach (var nestedType in type.GetNestedTypes())
                    fields = fields.Concat(GetStaticFieldNames(nestedType, prefix));

                return fields;
            }
        }

        var userRepository = host.Services.GetRequiredService<IRepository<User>>();
        if (overwrite || !await userRepository.AnyAsync())
        {
            database.GetCollection<User>(userRepository.CollectionName).DeleteMany(FilterDefinition<User>.Empty);
            var creationTime = DateTimeOffset.UtcNow;
            await userRepository.AddAsync(new User
            {
                Id = "admin",
                Name = "Administrator",
                Secret = host.Services.GetRequiredService<IPasswordHandler>().Encrypt("super", "admin"),
                CreationTime = creationTime,
                LastLoginTime = creationTime,
                Roles = { "admin" },
                Chats = { "general", "admin_chat" },
            });
        }

        var chatRepository = host.Services.GetRequiredService<IRepository<Chat>>();
        if (overwrite || !await chatRepository.AnyAsync())
        {
            database.GetCollection<Chat>(chatRepository.CollectionName).DeleteMany(FilterDefinition<Chat>.Empty);
            await chatRepository.AddRangeAsync(new List<Chat>
            {
                new Chat
                {
                    Id = "general",
                    Name = "General",
                    Users = { "admin" }
                },
                new Chat
                {
                    Id = "admin_chat",
                    Name = "Admin Chat",
                    Users = { "admin" }
                }
            });
        }
    }
}
