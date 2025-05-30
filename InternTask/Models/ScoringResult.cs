namespace InternTask.Models;
public class ScoringResult
{
  
    public bool IsApproved { get; set; }
        

    public decimal EligibleAmount { get; set; } = 0m;

 
    public string Message { get; set; }
        
  
    public List<ConditionEvaluationDetail> ConditionResults { get; set; } = new List<ConditionEvaluationDetail>();
}