using Prometheus;
using System;
using System.Collections.Generic;

namespace InternTask.Services
{
    public class ScoringMetricsService
    {
        private readonly Counter _totalScoringRequests;
        private readonly Counter _successfulScoringRequests;
        private readonly Counter _failedScoringRequests;
        
      
        private readonly Counter _conditionErrors;
        
       
        private readonly Histogram _conditionEvaluationDuration;
        
       
        public ScoringMetricsService()
        {
            _totalScoringRequests = Metrics.CreateCounter(
                "scoring_requests_total",
                "Total number of scoring requests processed");
                
            _successfulScoringRequests = Metrics.CreateCounter(
                "scoring_requests_successful_total",
                "Number of scoring requests that resulted in approval");
                
            _failedScoringRequests = Metrics.CreateCounter(
                "scoring_requests_failed_total",
                "Number of scoring requests that resulted in rejection");
                
            _conditionErrors = Metrics.CreateCounter(
                "scoring_condition_errors_total", 
                "Number of errors during condition evaluation",
                new CounterConfiguration
                {
                    LabelNames = new[] { "condition_name" }
                });
                
       
            _conditionEvaluationDuration = Metrics.CreateHistogram(
                "scoring_condition_evaluation_seconds",
                "Time spent evaluating conditions in seconds",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "condition_name" },
                    Buckets = new[] { 0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1, 5 } // durations in seconds
                });
        }
        
        public void RecordScoringRequest()
        {
            _totalScoringRequests.Inc();
        }
        
        public void RecordSuccessfulScoringRequest()
        {
            _successfulScoringRequests.Inc();
        }
        
        public void RecordFailedScoringRequest()
        {
            _failedScoringRequests.Inc();
        }
        
        public void RecordConditionError(string conditionName)
        {
            _conditionErrors.WithLabels(conditionName).Inc();
        }
        
        public IDisposable MeasureConditionDuration(string conditionName)
        {
            return _conditionEvaluationDuration.WithLabels(conditionName).NewTimer();
        }
    }
}