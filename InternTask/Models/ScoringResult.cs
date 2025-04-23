namespace InternTask.Models;
public class ScoringResult
{
    /// <summary>
    /// Whether the customer passed all required conditions
    /// </summary>
    public bool IsApproved { get; set; }
        
    /// <summary>
    /// The final eligible loan amount, if approved
    /// </summary>
    public decimal? EligibleAmount { get; set; }
        
    /// <summary>
    /// Summary message explaining the scoring result
    /// </summary>
    public string Message { get; set; }
        
    /// <summary>
    /// Detailed results for each condition that was evaluated
    /// </summary>
    public List<ConditionEvaluationDetail> ConditionResults { get; set; } = new List<ConditionEvaluationDetail>();
}