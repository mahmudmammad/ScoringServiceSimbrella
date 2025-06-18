using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using InternTask.Entities;
using InternTask.Models.Configuration;
using Microsoft.Extensions.Options;

namespace InternTask.DB
{
    public class ScoringDbContext : DbContext
    {
        private readonly ConditionsConfig _conditionConfig;

        public ScoringDbContext(DbContextOptions<ScoringDbContext> options, IOptions<ConditionsConfig> conditionOptions)
            : base(options)
        {
            _conditionConfig = conditionOptions.Value;
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



        }
    }
}