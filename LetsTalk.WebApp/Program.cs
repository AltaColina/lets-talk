using Duende.Bff.Yarp;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddContainersConfiguration("localhost");
var webApiUrl = builder.Configuration.GetConnectionString("LetsTalk.WebApi");
if (String.IsNullOrEmpty(webApiUrl))
    throw new InvalidOperationException("Invalid WebApi URL");

builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
    options.DefaultSignOutScheme = "oidc";
}).AddCookie("cookie", options =>
{
    options.Cookie.Name = "__Host-bff";
    options.Cookie.SameSite = SameSiteMode.Strict;
}).AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://demo.duendesoftware.com";
    options.ClientId = "interactive.confidential";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.ResponseMode = "query";

    options.GetClaimsFromUserInfoEndpoint = true;
    options.MapInboundClaims = false;
    options.SaveTokens = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");

    options.TokenValidationParameters = new()
    {
        NameClaimType = "name",
        RoleClaimType = "role"
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

app.MapRemoteBffApiEndpoint("/hubs/letstalk", $"{webApiUrl.Replace("https", "wss")}/hubs/letstalk")
    .SkipResponseHandling()
    .SkipAntiforgery();

app.MapFallbackToFile("index.html");

app.Run();
