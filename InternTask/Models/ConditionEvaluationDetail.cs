namespace InternTask.Models;

public class ConditionEvaluationDetail
{
    /// <summary>
    /// ID of the condition
    /// </summary>
    public string ConditionId { get; set; }
        
    /// <summary>
    /// Name of the condition
    /// </summary>
    public string ConditionName { get; set; }
        
    /// <summary>
    /// Whether this condition is required to pass
    /// </summary>
    public bool IsRequired { get; set; }
        
    /// <summary>
    /// The original result of the condition evaluation
    /// </summary>
    public ConditionResult  Result { get; set; }
}