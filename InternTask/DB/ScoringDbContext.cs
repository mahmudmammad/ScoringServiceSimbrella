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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConditionEvaluationEntity>()
                .Property(c => c.ParametersJson)
                .HasColumnType("jsonb");
                
            modelBuilder.Entity<ConditionEvaluationEntity>()
                .HasOne(c => c.ScoringResult)
                .WithMany(s => s.ConditionEvaluations)
                .HasForeignKey(c => c.ScoringResultEntityId);
        }
    }
}