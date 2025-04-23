using InternTask.Models;

namespace InternTask.Interfaces;

public interface ICondition
{
    /// <summary>
    /// Unique identifier for the condition
    /// </summary>
    string Id { get; }
        
    /// <summary>
    /// Human-readable name for the condition
    /// </summary>
    string Name { get; }
        
    /// <summary>
    /// Priority of the condition in the evaluation sequence
    /// </summary>
    int Priority { get; }
        
    /// <summary>
    /// Indicates if this is a required condition that must pass
    /// </summary>
    bool IsRequired { get; }
        
    /// <summary>
    /// Evaluates the condition against the provided customer
    /// </summary>
    /// <param name="customer">The customer to evaluate</param>
    /// <returns>The result of the condition evaluation</returns>
    ConditionResult Evaluate(Customer customer);
}