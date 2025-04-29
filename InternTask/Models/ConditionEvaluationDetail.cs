namespace InternTask.Models;

public class ConditionEvaluationDetail
{
  
    public int ConditionId { get; set; } 
        
  
    public string ConditionName { get; set; }
        
    
    public bool IsRequired { get; set; }

    public ConditionResult  Result { get; set; }
}