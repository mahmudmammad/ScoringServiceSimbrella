using InternTask.Factories;
using InternTask.Interfaces;
using InternTask.Services;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Load and bind configuration (appsettings.json)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(); // Logs will still appear in console
});

builder.Services.AddSingleton<IScoringService, ScoringService>();

// Use factory to register conditions
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("ConditionFactory");

    return ConditionFactory.CreateFromConfiguration(config, logger);
});

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();