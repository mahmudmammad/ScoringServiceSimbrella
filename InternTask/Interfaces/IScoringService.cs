using InternTask.Models;

namespace InternTask.Interfaces;

public interface IScoringService
{
    Task<ScoringResult> EvaluateCustomerAsync(Customer customer);
}