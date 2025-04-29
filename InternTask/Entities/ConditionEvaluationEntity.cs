namespace InternTask.Entities;

public class ConditionEvaluationEntity
{
    public int Id { get; set; }



    public string ConditionName { get; set; }

    public bool IsRequired { get; set; }

    public bool Passed { get; set; }

    public decimal? EligibleAmount { get; set; }

    public string Message { get; set; }

    public int ScoringResultEntityId { get; set; }

    public ScoringResultEntity ScoringResult { get; set; }
}
