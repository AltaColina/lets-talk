using FluentValidation;
using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LiteDB;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;
public sealed class LiteDbOptions
{
    public ConnectionString ConnectionString { get; set; } = new ConnectionString();
    public BsonMapper BsonMapper { get; set; } = BsonMapper.Global.UseCamelCase();
}

public static class DependencyInjection
{
    public static IServiceCollection AddLiteDb(this IServiceCollection services, Action<LiteDbOptions>? configure = null)
    {
        var options = new LiteDbOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(provider =>
        {
            var passwordHandler = provider.GetRequiredService<IPasswordHandler>();
            var database = new LiteDatabase(options.ConnectionString, options.BsonMapper);
            var roles = database.GetCollection<Role>();
            if (!database.CollectionExists(nameof(Role)))
            {
                var allPermissions = GetStaticFieldNames(typeof(Permissions));
                database.GetCollection<Role>().InsertBulk(new List<Role>
                {
                    new Role
                    {
                        Id = "User",
                        Permissions =
                        {
                            Permissions.Chat.View,
                            Permissions.Role.View,
                            Permissions.User.View,
                        }
                    },
                    new Role
                    {
                        Id = "Admin",
                        Permissions = new List<string>(allPermissions)
                    },
                },
                batchSize: 3);
            }
            if (!database.CollectionExists(nameof(User)))
            {
                var creationTime = DateTimeOffset.UtcNow;
                database.GetCollection<User>().Insert(new User
                {
                    Id = "admin",
                    Secret = passwordHandler.Encrypt("super", "admin"),
                    CreationTime = creationTime,
                    LastLoginTime = creationTime,
                    Roles = { "Admin" }
                });
            }
            if (!database.CollectionExists(nameof(Chat)))
            {
                database.GetCollection<Chat>().Insert(new Chat
                {
                    Id = "General"
                });
            }
            return database;
        });
        return services;

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

    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddMediatR(handlerAssemblyMarkerTypes);
        return services;
    }

    public static IServiceCollection AddFluentValidation(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
    {
        foreach (var type in handlerAssemblyMarkerTypes)
            services.AddValidatorsFromAssembly(type.Assembly);
        return services;
    }
}
