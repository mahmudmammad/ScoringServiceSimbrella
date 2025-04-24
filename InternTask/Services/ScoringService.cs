using InternTask.Interfaces;
using InternTask.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternTask.Services
{

    public class ScoringService : IScoringService
    {
        private readonly IEnumerable<ICondition> _conditions;
        private readonly ILogger<ScoringService> _logger;


        public ScoringService(IEnumerable<ICondition> conditions, ILogger<ScoringService> logger)
        {
            _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

     
        public async Task<ScoringResult> EvaluateCustomerAsync(Customer customer)
        {
            _logger.LogInformation($"Starting evaluation for customer ID: {customer?.Id}");

            if (customer == null)
            {
                _logger.LogWarning("Customer object is null");
                return new ScoringResult
                {
                    IsApproved = false,
                    EligibleAmount = null,
                    Message = "Customer information is missing"
                };
            }

            ScoringResult result = new ScoringResult();
            bool failedRequiredConditions = false;
            List<decimal> eligibleAmounts = new List<decimal>();

   
            List<ICondition> sortedConditions = _conditions.OrderBy(c => c.Priority).ToList();

            foreach (var condition in sortedConditions)
            {
                try
                {
                    _logger.LogDebug($"Evaluating condition: {condition.Id} ({condition.Name})");
                    
         
                    var conditionResult = condition.Evaluate(customer);
                    
                    ConditionEvaluationDetail detail = new ConditionEvaluationDetail
                    {
                        ConditionId = condition.Id,
                        ConditionName = condition.Name,
                        IsRequired = condition.IsRequired,
                        Result = conditionResult
                    };
                    
       
                    result.ConditionResults.Add(detail);
                    
    
                    _logger.LogInformation(
                        $"Condition {condition.Id} evaluation: {(conditionResult.Passed ? "Passed" : "Failed")}. " +
                        $"Message: {conditionResult.Message}"
                    );

  
                    if (!conditionResult.Passed && condition.IsRequired)
                    {
                        _logger.LogWarning($"Required condition {condition.Id} failed");
                        failedRequiredConditions = true;
                        
         
                        if (failedRequiredConditions)
                        {
                            break;
                        }
                    }

       
                    if (conditionResult.Passed && conditionResult.EligibleAmount.HasValue)
                    {
                        eligibleAmounts.Add(conditionResult.EligibleAmount.Value);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error evaluating condition {condition.Id}: {ex.Message}");
                    
                    ConditionResult errorResult = ConditionResult.Failure($"Error during evaluation: {ex.Message}");
                    
        
                    result.ConditionResults.Add(new ConditionEvaluationDetail
                    {
                        ConditionId = condition.Id,
                        ConditionName = condition.Name,
                        IsRequired = condition.IsRequired,
                        Result = errorResult
                    });
                    
              
                    if (condition.IsRequired)
                    {
                        failedRequiredConditions = true;
                        break;
                    }
                }
            }

     
            result.IsApproved = !failedRequiredConditions && result.ConditionResults.Any(r => r.Result.Passed);
            
            if (result.IsApproved && eligibleAmounts.Any())
            {
  
                result.EligibleAmount = eligibleAmounts.Min();
                result.Message = $"Customer approved for a loan up to {result.EligibleAmount:C}";
            }
            else if (result.IsApproved)
            {
                result.Message = "Customer approved but no specific amount determined";
            }
            else
            {
                result.Message = "Customer not approved";
            }

            _logger.LogInformation(
                $"Evaluation completed for customer ID: {customer.Id}. " +
                $"Result: {(result.IsApproved ? "Approved" : "Not Approved")}. " +
                $"Eligible amount: {(result.EligibleAmount.HasValue ? result.EligibleAmount.Value.ToString("C") : "N/A")}"
            );

            
            return await Task.FromResult(result);
        }
    }
}