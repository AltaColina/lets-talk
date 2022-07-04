using LetsTalk.Behaviors;
using LetsTalk.Interfaces;
using LetsTalk.Models;
using LetsTalk.Services;
using LiteDB;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("SecurityKey").Value));
builder.Services.AddSingleton<SecurityKey>(securityKey);
builder.Services.AddSingleton<HashAlgorithm>(HashAlgorithm.Create(builder.Configuration.GetSection("HashAlgorithm").Value)!);
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
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Administrators", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));
});
builder.Services.AddSingleton(services =>
{
    var passwordHandler= services.GetRequiredService<IPasswordHandler>();
    var database = new LiteDatabase(builder.Configuration.GetConnectionString("LiteDB"), BsonMapper.Global.UseCamelCase());
    if (!database.CollectionExists(nameof(User)))
    {
        var creationTime = DateTimeOffset.UtcNow;
        database.GetCollection<User>().Insert(new User
        {
            Id = "admin",
            Secret = passwordHandler.Encrypt("super", "admin"),
            CreationTime = creationTime,
            LastLoginTime = creationTime,
            Roles =
            {
                Role.User,
                Role.Moderator,
                Role.Administrator
            }
        });
    }
    if (!database.CollectionExists(nameof(Chat)))
    {
        database.GetCollection<Chat>().Insert(new Chat
        {
            Id = Guid.NewGuid().ToString(),
            Name = "General"
        });
    }
    return database;
});
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();

builder.Services.AddSingleton<IPasswordHandler, PasswordHandler>();
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IAuthenticationManager, AuthenticationManager>();

builder.Services.AddMediatR(typeof(Program), typeof(LetsTalk.Shared.IAssemblyMarker));

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
