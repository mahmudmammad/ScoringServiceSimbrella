using InternTask.Models;

namespace InternTask.Interfaces;

public interface IScoringService
{
    /// <summary>
    /// Evaluates a customer against all registered scoring conditions
    /// </summary>
    /// <param name="customer">The customer to evaluate</param>
    /// <returns>The scoring result with approval status and eligible amount</returns>
    Task<ScoringResult> EvaluateCustomerAsync(Customer customer);
}