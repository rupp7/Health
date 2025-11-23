using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

// Basic lifetime logging to help diagnose unexpected shutdowns
var lifetime = app.Lifetime;
lifetime.ApplicationStopping.Register(() =>
{
	Console.WriteLine($"ApplicationStopping at {DateTime.UtcNow:o}");
});
lifetime.ApplicationStopped.Register(() =>
{
	Console.WriteLine($"ApplicationStopped at {DateTime.UtcNow:o}");
});

// simple root endpoint so GET / does not return 404 (useful for quick smoke tests)
app.MapGet("/", () => Results.Text("Health.Api running"));

app.MapControllers();

try
{
	app.Run();
}
catch (Exception ex)
{
	// log and rethrow so container/host shows error
	Console.WriteLine($"Host terminated unexpectedly: {ex}");
	throw;
}
