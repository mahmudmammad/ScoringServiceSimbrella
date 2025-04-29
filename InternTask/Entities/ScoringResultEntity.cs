namespace InternTask.Entities;

public class ScoringResultEntity
{
    public int Id { get; set; }

    public bool IsApproved { get; set; }

    public decimal? EligibleAmount { get; set; }

    public string Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ConditionEvaluationEntity> ConditionEvaluations { get; set; } = new();
}
