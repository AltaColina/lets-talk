using LetsTalk.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Logging.
builder.Host.UseSerilog((host, services, config) => config
    .ReadFrom.Configuration(host.Configuration));

// TODO: Remove this.
builder.Services.AddCryptography(builder.Configuration);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

// Identity.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IUserStore, UserStore>();
builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = "https://identity";
    // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
    options.EmitStaticAudienceClaim = true;
})
    .AddResourceStore<ResourceStore>()
    .AddClientStore<ClientStore>()
    .AddProfileService<UserProfileService>()
    .AddResourceOwnerValidator<UserResourceOwnerPasswordValidator>()
    .AddBackchannelAuthenticationUserValidator<BackchannelAuthenticationUserValidator>();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

await app.InsertResourceDataAsync(app.Configuration, overwrite: true);
await app.InsertUserDataAsync(app.Configuration, overwrite: true);

app.Run();