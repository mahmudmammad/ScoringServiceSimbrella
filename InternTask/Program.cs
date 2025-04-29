using InternTask.DB;
using InternTask.Factories;
using InternTask.Interfaces;
using InternTask.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDbContext<ScoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddScoped<IScoringService, ScoringService>();


builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("ConditionFactory");

    return ConditionFactory.CreateFromConfiguration(config, logger);
});


builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();