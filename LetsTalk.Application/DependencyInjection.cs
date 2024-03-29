﻿using CommunityToolkit.Mvvm.Messaging;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentValidation;
using LetsTalk.Behaviors;
using LetsTalk.Profiles;
using LetsTalk.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var currentAsm = typeof(DependencyInjection).Assembly;
        var callingAsm = Assembly.GetCallingAssembly();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddAutoMapper(config => config.AddProfile(new AutoMapperProfile(currentAsm, callingAsm)));
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblies(currentAsm, callingAsm));
        services.AddValidatorsFromAssembly(currentAsm);
        services.AddValidatorsFromAssembly(callingAsm);
        services.AddScoped(typeof(IValidatorService<>), typeof(ValidatorService<>));
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        return services;
    }

    public static IConfigurationBuilder AddContainersConfiguration(this IConfigurationBuilder configuration, string hostname)
    {
        var dockerClient = new DockerClientConfiguration().CreateClient();
        var containers = Task.Run(async () => await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true })).Result;
        var connectionStrings = new Dictionary<string, string?>();
        foreach (var container in containers)
        {
            var ports = container.Ports.Where(p => p.PublicPort > 0).ToList();
            if (ports.Count > 0)
            {
                var port = ports.Count == 1
                    ? ports[0]
                    : ports.Find(p => p.PrivatePort == 443 /*https*/) ?? ports[0];
                var containerName = container.Names[0][1..];
                connectionStrings[$"ConnectionStrings:{containerName}"] = $"https://{hostname}:{port.PublicPort}";
            }
        }
        configuration.AddInMemoryCollection(connectionStrings);
        return configuration;
    }

    public static IServiceCollection AddLetsTalkHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<ILetsTalkHttpClient, LetsTalkHttpClient>(opts => opts.BaseAddress = new(configuration.GetConnectionString("LetsTalk.WebApi")!));
        return services;
    }

    public static IServiceCollection AddLetsTalkHubClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ILetsTalkHubClient, LetsTalkHubClient>();
        return services;
    }
}