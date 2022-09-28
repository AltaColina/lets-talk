using LetsTalk;
using LetsTalk.Filters;
using LetsTalk.Hubs;
using LetsTalk.Interfaces;
using LetsTalk.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Security.
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("SecurityKey").Value));
builder.Services.AddSingleton(HashAlgorithm.Create(builder.Configuration.GetSection("HashAlgorithm").Value)!);
builder.Services.AddSingleton<IPasswordHandler, PasswordHandler>();
builder.Services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
builder.Services.AddSingleton<SecurityKey>(securityKey);
builder.Services.AddSingleton<ITokenProvider, JwtTokenProvider>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.SaveToken = true;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });
builder.Services.AddSingleton<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

builder.Services.AddApplication(builder.Configuration);
 
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

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(opts => opts.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();
app.MapHub<LetsTalkHub>("/letstalk", opts => opts.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);

await app.LoadDatabaseData(overwrite: true);

app.Run();
