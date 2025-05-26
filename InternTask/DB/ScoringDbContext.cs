using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using InternTask.Entities;

namespace InternTask.DB
{
    public class ScoringDbContext : DbContext
    {
        public ScoringDbContext(DbContextOptions<ScoringDbContext> options) : base(options)
        {
        }

        public DbSet<ScoringResultEntity> ScoringResults { get; set; }
        public DbSet<ConditionEvaluationEntity> ConditionEvaluations { get; set; }
        public DbSet<ConditionConfigEntity> ConditionConfigs { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configure ConditionEvaluationEntity
    modelBuilder.Entity<ConditionEvaluationEntity>()
        .Property(c => c.ParametersJson)
        .HasColumnType("jsonb");

    modelBuilder.Entity<ConditionEvaluationEntity>()
        .HasOne(c => c.ScoringResult)
        .WithMany(s => s.ConditionEvaluations)
        .HasForeignKey(c => c.ScoringResultEntityId);

    // Configure ConditionConfigEntity
    modelBuilder.Entity<ConditionConfigEntity>()
        .Property(c => c.ParametersJson)
        .HasColumnType("jsonb");

    modelBuilder.Entity<ConditionConfigEntity>().HasData(
        new ConditionConfigEntity
        {
            Id = 1,
            Type = "SalaryThreshold",
            Enabled = true,
            Required = true,
            Priority = 1,
            ParametersJson = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "MinimumSalary", "5000" },
                { "SalaryMultiplier", "1.2" }
            })
        },
        new ConditionConfigEntity
        {
            Id = 2,
            Type = "LoanCountLimit",
            Enabled = true,
            Required = true,
            Priority = 2,
            ParametersJson = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "MaxLoanCount", "5" }
            })
        },
        new ConditionConfigEntity
        {
            Id = 3,
            Type = "AccountBalance",
            Enabled = true,
            Required = false,
            Priority = 3,
            ParametersJson = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "MinimumBalance", "1000" },
                { "BalanceMultiplier", "2.0" }
            })
        },
        new ConditionConfigEntity
        {
            Id = 4,
            Type = "DefaultHistory",
            Enabled = false,
            Required = true,
            Priority = 4,
            ParametersJson = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "FailIfHasDefault", "true" }
            })
        }
    );
}

    }
}