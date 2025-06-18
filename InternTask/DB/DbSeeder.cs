// Services/DbSeeder.cs

using System.Text.Json;
using InternTask.Entities;
using InternTask.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InternTask.DB;

public static class DbSeeder
{
    public static async Task SeedConditionConfigsAsync(ScoringDbContext dbContext, ConditionsConfig config)
    {
        if (await dbContext.ConditionConfigs.AnyAsync())
            return; 

        var entities = config.Conditions.Select((c, index) => new ConditionConfigEntity
        {
            Id = index + 1,
            Type = c.Type,
            Enabled = c.Enabled,
            Required = c.Required,
            Priority = c.Priority,
            ParametersJson = JsonSerializer.Serialize(c.Parameters)
        });

        dbContext.ConditionConfigs.AddRange(entities);
        await dbContext.SaveChangesAsync();
    }
}