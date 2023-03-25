using IdentityModel;
using LetsTalk.Filters;
using LetsTalk.Hubs;
using LetsTalk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Logging.
builder.Host.UseSerilog((host, services, config) => config
    .ReadFrom.Configuration(host.Configuration)
    .Destructure.With<SensitiveDataDestructuringPolicy>());
// Security.
builder.Services.AddCryptography(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
    {
        opts.Authority = builder.Configuration.GetConnectionString("Identity");
        //options.Audience = "api";

        //options.MapInboundClaims = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = JwtClaimTypes.Role,
            NameClaimType = JwtClaimTypes.Name,
            ValidateAudience = false,
        };
        opts.BackchannelHttpHandler = new HttpClientHandler()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyErrors) => true
        };
        opts.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    var accessToken = context.Request.Query["access_token"];
                    if (!String.IsNullOrEmpty(accessToken))
                        context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR()
    .AddMessagePackProtocol();
builder.Services.AddControllers(opts => opts.Filters.Add<HttpExceptionFilter>())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SupportNonNullableReferenceTypes();
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme (token only)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    };
    opts.OperationFilter<AuthorizeOperationFilter>();
    opts.AddSecurityDefinition("bearer", securitySchema);
});

// Application.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<LetsTalkHub>("/hubs/letstalk", opts => opts.Transports = HttpTransportType.WebSockets);

app.Run();
