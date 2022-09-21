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
        services.AddStackExchangeRedisCache(opts => opts.Configuration = "cache:6379");
        services.AddSingleton<IHubConnectionMapper, HubConnectionMapper>();
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
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(opts => opts.BaseAddress = new(configuration.GetSection("LetsTalkRestAddress").Value));
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
                        Permissions = new List<string>(allPermissions.Where(perm => perm.EndsWith("View", StringComparison.InvariantCultureIgnoreCase)))
                    },
                    new Role
                    {
                        Id = "admin",
                        Name = "Administrator",
                        Permissions = new List<string>(allPermissions)
                    },
                });

            static IEnumerable<string> GetStaticFieldNames(Type type, string prefix = "")
            {
                prefix += $"{type.Name}.";

                var fields = type
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(field => $"{prefix}{field.Name}");

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
                Secret = host.Services.GetRequiredService<IPasswordHandler>().Encrypt("super", "admin"),
                CreationTime = creationTime,
                LastLoginTime = creationTime,
                Roles = { "admin" },
                Chats = { "general" },
            });
        }

        var chatRepository = host.Services.GetRequiredService<IRepository<Chat>>();
        if (overwrite || !await chatRepository.AnyAsync())
        {
            database.GetCollection<Chat>(chatRepository.CollectionName).DeleteMany(FilterDefinition<Chat>.Empty);
            await chatRepository.AddAsync(new Chat
            {
                Id = "general",
                Name = "General",
            });
        }
    }
}
