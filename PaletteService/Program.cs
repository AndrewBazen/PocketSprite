/* Program.cs
*
* @description: Main entry point for the PaletteService
*
* @author: Andrew Bazen <github.com/AndrewBazen>
* @version: 1.0
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*/
using PaletteService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the DbContext
DbContextConfig.Configure(builder.Services, builder.Configuration);

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Use HTTPS redirection only in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Use authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();
