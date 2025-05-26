using InternTask.Conditions;
using InternTask.Interfaces;
using InternTask.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using InternTask.DB;

namespace InternTask.Factories
{

    public static class ConditionFactory
    {

        public static IEnumerable<ICondition> CreateFromConfiguration(
            IConfiguration configuration,
            ILogger logger)
        {
            List<ICondition> conditions = new List<ICondition>();
            
         
            ConditionsConfig conditionsConfig = new ConditionsConfig();
            configuration.GetSection("ScoringService").Bind(conditionsConfig);
            
            logger.LogInformation($"Loading {conditionsConfig.Conditions.Count} conditions from configuration");
            
            foreach (ConditionConfig config in conditionsConfig.Conditions)
            {
                if (!config.Enabled)
                {
                    logger.LogInformation($"Skipping disabled condition: {config.Type}");
                    continue;
                }
                
                try
                {
                    ICondition condition = CreateCondition(config);
                    
                    if (condition != null)
                    {
                        conditions.Add(condition);
                        logger.LogInformation($"Created condition: {condition.Id} (Priority: {condition.Priority})");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error creating condition {config.Type}: {ex.Message}");
                }
            }
            
            return conditions;
        }
        
  
        private static ICondition CreateCondition(ConditionConfig config)
        {
            switch (config.Type)
            {
                case "SalaryThreshold":
                    decimal minimumSalary = GetDecimalParameter(config.Parameters, "MinimumSalary", 3000);
                    decimal salaryMultiplier = GetDecimalParameter(config.Parameters, "SalaryMultiplier", 3.0m);
                    return new SalaryThresholdCondition(minimumSalary, salaryMultiplier);
                    
                case "LoanCountLimit":
                    int maxLoanCount = GetIntParameter(config.Parameters, "MaxLoanCount", 3);
                    return new LoanCountLimitCondition(maxLoanCount);
                    
                case "AccountBalance":
                    decimal minimumBalance = GetDecimalParameter(config.Parameters, "MinimumBalance", 1000);
                    decimal balanceMultiplier = GetDecimalParameter(config.Parameters, "BalanceMultiplier", 2.0m);
                    return new AccountBalanceCondition(minimumBalance, balanceMultiplier);
                case "DefaultHistory":
                    bool failIfHasDefault = GetBoolParameter(config.Parameters, "FailIfHasDefault", true);
                    return new HasDefaultHistoryCondition(failIfHasDefault);   
     
                    
                default:
                    throw new ArgumentException($"Unknown condition type: {config.Type}");
            }
        }
        
        public static IEnumerable<ICondition> CreateFromDatabase(
            ScoringDbContext dbContext,
            ILogger logger)
        {
            List<ICondition> conditions = new List<ICondition>();

            var dbConfigs = dbContext.ConditionConfigs
                .Where(c => c.Enabled)
                .ToList();

            logger.LogInformation($"Loading {dbConfigs.Count} conditions from database");

            foreach (var configEntity in dbConfigs)
            {
                try
                {
                    Dictionary<string, string> parameters = new();

                    if (!string.IsNullOrWhiteSpace(configEntity.ParametersJson))
                    {
                        parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(configEntity.ParametersJson);
                    }

                    var config = new Models.Configuration.ConditionConfig
                    {
                        Type = configEntity.Type,
                        Enabled = configEntity.Enabled,
                        Parameters = parameters
                    };

                    ICondition condition = CreateCondition(config);

                    if (condition != null)
                    {
                        conditions.Add(condition);
                        logger.LogInformation($"Created condition from DB: {condition.Id} (Priority: {condition.Priority})");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error creating condition {configEntity.Type}: {ex.Message}");
                }
            }

            return conditions;
        }
        

        private static decimal GetDecimalParameter(Dictionary<string, string> parameters, string name, decimal defaultValue)
        {
            if (parameters != null && parameters.TryGetValue(name, out string value))
            {
                if (decimal.TryParse(value, out decimal result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        

        private static int GetIntParameter(Dictionary<string, string> parameters, string name, int defaultValue)
        {
            if (parameters != null && parameters.TryGetValue(name, out string value))
            {
                if (int.TryParse(value, out int result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
        
      
        private static bool GetBoolParameter(Dictionary<string, string> parameters, string name, bool defaultValue)
        {
            if (parameters != null && parameters.TryGetValue(name, out string value))
            {
                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }
            }
            return defaultValue;
        }
    }
}