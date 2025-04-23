using InternTask.Conditions;
using InternTask.Factories;
using InternTask.Interfaces;
using InternTask.Models;
using InternTask.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Create logger
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        ILogger<ScoringService> logger = loggerFactory.CreateLogger<ScoringService>();

        // Create conditions dynamically from config
        IEnumerable<ICondition> conditions = ConditionFactory.CreateFromConfiguration(configuration, logger);

        // Instantiate the scoring service
        var scoringService = new ScoringService(conditions, logger);

        // Create test customer
        var customer = new Customer
        {
            Id = 1,
            Salary = 42000,
            LoanCount = 4,
            AccountBalance = 4500,
            Age = 30,
            Gender = "Female",
            HasDefaultHistory = false
        };

        
        var result = await scoringService.EvaluateCustomerAsync(customer);

        Console.WriteLine("\n=== Evaluation Result ===");
        Console.WriteLine($"Approved: {result.IsApproved}");
        Console.WriteLine($"Eligible Amount: {(result.EligibleAmount.HasValue ? result.EligibleAmount.Value.ToString("C") : "N/A")}");
        Console.WriteLine($"Message: {result.Message}");
        Console.WriteLine("\n--- Details ---");
        foreach (var detail in result.ConditionResults)
        {
            Console.WriteLine($"{detail.ConditionName}: {(detail.Result.Passed ? "✔️" : "❌")} - {detail.Result.Message}");
        }
    }
}
