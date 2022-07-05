using FluentValidation;
using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Services;
using LiteDB;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Security.
var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("SecurityKey").Value));
builder.Services.AddSingleton<SecurityKey>(securityKey);
builder.Services.AddSingleton<HashAlgorithm>(HashAlgorithm.Create(builder.Configuration.GetSection("HashAlgorithm").Value)!);
builder.Services.AddSingleton<IPasswordHandler, PasswordHandler>();
builder.Services.AddSingleton<ITokenProvider, JwtTokenProvider>();

// Authentication.
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

// Authorization.
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

// Repositories.
builder.Services.AddLiteDb(opts => opts.ConnectionString.Filename = builder.Configuration.GetConnectionString("LiteDB"));
builder.Services.AddSingleton<IRoleRepository, RoleRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();

// Infrastructure.
builder.Services.AddMediator(typeof(Program), typeof(LetsTalk.Shared.IAssemblyMarker));
builder.Services.AddFluentValidation(typeof(Program));

builder.Services.AddSignalR(opts => opts.EnableDetailedErrors = true);
builder.Services.AddControllers(opts => opts.Filters.Add<HttpExceptionFilter>())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
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
app.MapHub<LetsTalkHub>("/letsTalk", opts => opts.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);

app.Run();
