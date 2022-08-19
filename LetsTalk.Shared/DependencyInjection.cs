using FluentValidation;
using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace LetsTalk;

public static class DependencyInjection
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

    public static IServiceCollection AddMongoDb(this IServiceCollection services, string connectionString)
    {
        services.TryAddSingleton<IMongoClient>(new MongoClient(connectionString));
        services.TryAddSingleton<IMongoDatabase>(provider =>
        {
            var database = provider.GetRequiredService<IMongoClient>().GetDatabase("letstalk");
            var collectionNames = database.ListCollectionNames().ToList();
            if (!collectionNames.Contains(nameof(Role)))
            {
                var allPermissions = GetStaticFieldNames(typeof(Permissions));
                database.CreateCollection(nameof(Role));
                database.GetCollection<Role>(nameof(Role)).InsertMany(new List<Role>
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

            if (!collectionNames.Contains(nameof(User)))
            {
                var creationTime = DateTimeOffset.UtcNow;
                database.CreateCollection(nameof(User));
                database.GetCollection<User>(nameof(User)).InsertOne(new User
                {
                    Id = "admin",
                    Secret = provider.GetRequiredService<IPasswordHandler>().Encrypt("super", "admin"),
                    CreationTime = creationTime,
                    LastLoginTime = creationTime,
                    Roles = { "Admin" }
                });
            }

            if (!collectionNames.Contains(nameof(Chat)))
            {
                database.CreateCollection(nameof(Chat));
                database.GetCollection<Chat>(nameof(Chat)).InsertOne(new Chat
                {
                    Id = "General"
                });
            }


            return database;
        });
        services.TryAddSingleton(typeof(IRepository<>), typeof(MongoRepository<>));

        return services;
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
