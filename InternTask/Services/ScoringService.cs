using InternTask.Interfaces;
using InternTask.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InternTask.DB;
using InternTask.Entities;

namespace InternTask.Services
{
    public class ScoringService : IScoringService
    {
        private readonly IEnumerable<ICondition> _conditions;
        private readonly ILogger<ScoringService> _logger;
        private readonly ScoringDbContext _context;
        private readonly ScoringMetricsService _metrics;

        public ScoringService(
            IEnumerable<ICondition> conditions, 
            ILogger<ScoringService> logger, 
            ScoringDbContext context,
            ScoringMetricsService metrics)
        {
            _conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        }

        public async Task<ScoringResult> EvaluateCustomerAsync(Customer customer)
        {
            
            _metrics.RecordScoringRequest();
            
            _logger.LogInformation($"Starting evaluation for customer");

            if (customer == null)
            {
                _logger.LogWarning("Customer object is null");
                _metrics.RecordFailedScoringRequest();
                return new ScoringResult
                {
                    IsApproved = false,
                    EligibleAmount = 0,
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
                    _logger.LogDebug($"Evaluating condition: ({condition.Name})");
              
                    using (_metrics.MeasureConditionDuration(condition.Name))
                    {
                        var conditionResult = condition.Evaluate(customer);
                        
                        ConditionEvaluationDetail detail = new ConditionEvaluationDetail
                        {
                            ConditionName = condition.Name,
                            IsRequired = condition.IsRequired,
                            Result = conditionResult,
                            Config = condition.GetConfiguration()
                        };
                        
                        result.ConditionResults.Add(detail);
                        
                        _logger.LogInformation(
                            $"Condition {condition.Name} evaluation: {(conditionResult.Passed ? "Passed" : "Failed")}. " +
                            $"Message: {conditionResult.Message}"
                        );

                        if (!conditionResult.Passed && condition.IsRequired)
                        {
                            _logger.LogWarning($"Required condition {condition.Name} failed");
                            failedRequiredConditions = true;
                            
                            if (failedRequiredConditions)
                            {
                                break;
                            }
                        }

                        if (conditionResult.Passed && conditionResult.EligibleAmount > 0)
                        {
                            eligibleAmounts.Add(conditionResult.EligibleAmount);
                        }

                    }
                }
                catch (Exception ex)
                {
                 
                    _metrics.RecordConditionError(condition.Name);
                    
                    _logger.LogError(ex, $"Error evaluating condition {condition.Id}: {ex.Message}");
                    
                    ConditionResult errorResult = ConditionResult.Failure($"Error during evaluation: {ex.Message}");
                    
                    result.ConditionResults.Add(new ConditionEvaluationDetail
                    {
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
       
                _metrics.RecordSuccessfulScoringRequest();
            }
            else if (result.IsApproved)
            {
                result.Message = "Customer approved but no specific amount determined";
                
                _metrics.RecordSuccessfulScoringRequest();
            }
            else
            {
                result.Message = "Customer not approved";
                
         
                _metrics.RecordFailedScoringRequest();
            }

            _logger.LogInformation(
                $"Evaluation completed for customer . " +
                $"Result: {(result.IsApproved ? "Approved" : "Not Approved")}. " +
                $"Eligible amount: {(result.EligibleAmount)}"
            );
            

            var scoringResultEntity = new ScoringResultEntity
            {
                IsApproved = result.IsApproved,
                EligibleAmount = result.EligibleAmount ,
                Message = result.Message,
                ConditionEvaluations = result.ConditionResults.Select(cr => new ConditionEvaluationEntity
                {
                    ConditionName = cr.ConditionName,
                    IsRequired = cr.IsRequired,
                    Passed = cr.Result.Passed,
                    EligibleAmount = cr.Result.EligibleAmount ,
                    Message = cr.Result.Message,
                   
                    Parameters = cr.Config ?? new Dictionary<string, object>()
                }).ToList()
            };

            _context.ScoringResults.Add(scoringResultEntity);
            await _context.SaveChangesAsync();
            
            return await Task.FromResult(result);
        }
    }
}