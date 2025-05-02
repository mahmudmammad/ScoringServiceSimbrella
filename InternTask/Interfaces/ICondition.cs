using InternTask.Models;

namespace InternTask.Interfaces;

public interface ICondition
{
    string Id { get; }
        
    
    string Name { get; }
        
   
    int Priority { get; }
        

    bool IsRequired { get; }
        

    ConditionResult Evaluate(Customer customer);
    
    Dictionary<string, object> GetConfiguration();
}