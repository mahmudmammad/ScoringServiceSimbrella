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
using Prometheus;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ScoringDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddSingleton<ScoringMetricsService>();

builder.Services.AddScoped<IScoringService, ScoringService>();
builder.Services.AddScoped<ConditionConfigService>();



// builder.Services.AddSingleton<IEnumerable<ICondition>>(sp => 
// {
//     var logger = sp.GetRequiredService<ILogger<Program>>();
//     return ConditionFactory.CreateFromConfiguration(builder.Configuration, logger);
// });

builder.Services.AddScoped<IEnumerable<ICondition>>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    var dbContext = sp.GetRequiredService<ScoringDbContext>();

    return ConditionFactory.CreateFromDatabase(dbContext, logger);
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ScoringDbContext>();
    dbContext.Database.Migrate();  
}

app.UseRouting();

app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();


app.Run();