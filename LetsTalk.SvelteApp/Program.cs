var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddContainersConfiguration("localhost");

builder.Services.AddLetsTalk(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapLetsTalk();

app.MapFallbackToFile("about", "about.html");
app.MapFallbackToFile("index.html");

app.Run();