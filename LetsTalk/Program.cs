using LetsTalk;
using LetsTalk.Filters;
using LetsTalk.Services;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Security.
var hashName = builder.Configuration.GetSection("HashAlgorithm").Value;
var securityKey = builder.Configuration.GetSection("SecurityKey").Value;
builder.Services.AddLetsTalkAuthentication(hashName, securityKey);
builder.Services.AddLetsTalkAuthorization();

// Repositories.
builder.Services.AddMongoDb(builder.Configuration.GetConnectionString("MongoDB"));

// Infrastructure.
builder.Services.AddMediator(typeof(Program), typeof(LetsTalk.IAssemblyMarker));
builder.Services.AddFluentValidation(typeof(Program));

builder.Services.AddSignalR();
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
