using System.Collections.Generic;

namespace InternTask.Models
{
    public class ConditionEvaluationDetail
    {
        public string ConditionName { get; set; }
        
        public bool IsRequired { get; set; }
        
        // Configuration parameters of the condition
        public Dictionary<string, object> Config { get; set; } = new Dictionary<string, object>();

        public ConditionResult Result { get; set; }
    }
}