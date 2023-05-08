using Duende.Bff.Yarp;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddContainersConfiguration("localhost");
var webApiUrl = builder.Configuration.GetConnectionString("LetsTalk.WebApi");
if (String.IsNullOrEmpty(webApiUrl))
    throw new InvalidOperationException("Invalid WebApi URL");

builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddAuthentication(options =>
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
    options.Authority = builder.Configuration.GetConnectionString("LetsTalk.Identity");
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
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpLogging();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
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

app.MapFallbackToFile("index.html");

app.Run();
