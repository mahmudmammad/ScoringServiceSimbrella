using InternTask.Factories;
using InternTask.Interfaces;
using InternTask.Services;
using InternTask.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database context
builder.Services.AddDbContext<ScoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Prometheus metrics

builder.Services.AddScoped<ScoringMetricsService>();
// Register ScoringService as scoped
builder.Services.AddScoped<IScoringService, ScoringService>();

// Register conditions from factory
builder.Services.AddSingleton<IEnumerable<ICondition>>(sp => 
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    return ConditionFactory.CreateFromConfiguration(builder.Configuration, logger);
});

var app = builder.Build();


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configure Prometheus metrics
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ScoringDbContext>();
    dbContext.Database.Migrate();  // Apply migrations automatically
}

app.UseAuthorization();
app.MapControllers();

app.Run();