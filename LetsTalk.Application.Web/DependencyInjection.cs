using Duende.Bff.Yarp;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddLetsTalk(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBff()
            .AddRemoteApis();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = "letstalk_webapp";
            options.Cookie.SameSite = SameSiteMode.Strict;
        }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = configuration.GetConnectionString("LetsTalk.Identity");
            options.ClientId = "web";
            options.ClientSecret = "secret";
            options.ResponseType = "code";
            options.ResponseMode = "query";

            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.SaveTokens = true;

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("letstalk");

            options.TokenValidationParameters = new()
            {
                RoleClaimType = JwtClaimTypes.Role,
                NameClaimType = JwtClaimTypes.Name,
                ValidateAudience = false,
            };
        });
        services.AddAuthorization();

        return services;
    }

    public static WebApplication MapLetsTalk(this WebApplication app)
    {
        var webApiUrl = app.Configuration.GetConnectionString("LetsTalk.WebApi");
        if (String.IsNullOrEmpty(webApiUrl))
            throw new InvalidOperationException("Invalid WebApi URL");

        app.UseRouting();
        app.UseAuthentication();
        app.UseBff();
        app.UseAuthorization();
        app.MapBffManagementEndpoints();

        app.MapRemoteBffApiEndpoint("/api", $"{webApiUrl}/api")
             .WithOptionalUserAccessToken()
             .AllowAnonymous();

        app.MapRemoteBffApiEndpoint("/hubs", $"{webApiUrl}/hubs")
            .RequireAccessToken()
            .SkipAntiforgery();

        return app;
    }
}