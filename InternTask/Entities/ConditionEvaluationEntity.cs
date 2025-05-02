using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

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
    
       
    [Column(TypeName = "jsonb")]
    public string ParametersJson { get; set; }
        
    [NotMapped]
    public Dictionary<string, object> Parameters 
    { 
        get => string.IsNullOrEmpty(ParametersJson) 
            ? new Dictionary<string, object>() 
            : JsonSerializer.Deserialize<Dictionary<string, object>>(ParametersJson);
        set => ParametersJson = JsonSerializer.Serialize(value);
    }

    public ScoringResultEntity ScoringResult { get; set; }
 
}
