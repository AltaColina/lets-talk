using FluentValidation;
using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Repositories;
using LetsTalk.Services;
using LiteDB;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class LetsTalkDependencyInjection
{
    public static IServiceCollection AddLetsTalkAuthentication(this IServiceCollection services, string hashName, string securityKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        var hash = HashAlgorithm.Create(hashName) ?? throw new ArgumentException($"Could not create HashAlgorithm '{hashName}'", nameof(hashName));
        services.TryAddSingleton(hash);
        services.TryAddSingleton<IPasswordHandler, PasswordHandler>();
        services.TryAddSingleton<IAuthenticationManager, AuthenticationManager>();
        services.TryAddSingleton<SecurityKey>(key);
        services.TryAddSingleton<ITokenProvider, JwtTokenProvider>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
        return services;
    }

    public static IServiceCollection AddLetsTalkAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        return services;
    }

    public static IServiceCollection AddLiteDb(this IServiceCollection services, Action<LiteDbOptions>? configure = null)
    {
        var options = new LiteDbOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(provider =>
        {
            var passwordHandler = provider.GetRequiredService<IPasswordHandler>();
            var database = new LiteDatabase(options.ConnectionString, options.BsonMapper);
            if (!database.CollectionExists(nameof(Role)))
            {
                var allPermissions = GetStaticFieldNames(typeof(Permissions));
                database.GetCollection<Role>().InsertBulk(new List<Role>
                {
                    new Role
                    {
                        Id = "User",
                        Permissions = new List<string>(allPermissions.Where(perm => perm.EndsWith("View", StringComparison.InvariantCultureIgnoreCase)))
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

        // Add related repositories.
        services.TryAddSingleton<IRoleRepository, RoleRepository>();
        services.TryAddSingleton<IUserRepository, UserRepository>();
        services.TryAddSingleton<IChatRepository, ChatRepository>();

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

    public static IServiceCollection AddLetsTalkHttpClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(configure);
        return services;
    }

    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
    {
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.TryAddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
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
