using InternTask.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternTask.DB;

public class ScoringDbContext : DbContext
{
    public ScoringDbContext(DbContextOptions<ScoringDbContext> options) : base(options) {}

    public DbSet<ScoringResultEntity> ScoringResults { get; set; }
    public DbSet<ConditionEvaluationEntity> ConditionEvaluations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScoringResultEntity>()
            .HasMany(sr => sr.ConditionEvaluations)
            .WithOne(c => c.ScoringResult)
            .HasForeignKey(c => c.ScoringResultEntityId);
    }
}
