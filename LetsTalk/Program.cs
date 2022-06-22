using LetsTalk.Services;
using LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("SecurityKey").Value));
builder.Services.AddSingleton<SecurityKey>(securityKey);
builder.Services.AddSingleton<HashAlgorithm>(HashAlgorithm.Create(builder.Configuration.GetSection("HashAlgorithm").Value)!);
builder.Services.AddSingleton<IPasswordHandler, PasswordHandler>();
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
builder.Services.AddAuthorization();
builder.Services.AddSingleton(new LiteDatabase(builder.Configuration.GetConnectionString("LiteDB"), BsonMapper.Global.UseCamelCase()));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IAuthenticationManager, JwtAuthenticationManager>();
builder.Services.AddGrpc();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<LetsTalkService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
