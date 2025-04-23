using InternTask.Interfaces;
using InternTask.Models;

namespace InternTask.Conditions
{
    /// <summary>
    /// Evaluates if a customer's existing loan count is below the maximum allowed
    /// </summary>
    public class LoanCountLimitCondition : ICondition
    {
        private readonly int _maxLoanCount;

        public string Id => "LoanCountLimit";
        public string Name => "Maximum Loan Count Check";
        public int Priority => 2;
        public bool IsRequired => true;

        public LoanCountLimitCondition(int maxLoanCount = 3)
        {
            _maxLoanCount = maxLoanCount;
        }

        public ConditionResult Evaluate(Customer customer)
        {
            if (customer == null)
            {
                return ConditionResult.Failure("Customer information is missing");
            }

            if (customer.LoanCount >= _maxLoanCount)
            {
                return ConditionResult.Failure(
                    $"Customer already has {customer.LoanCount} active loans, exceeding the maximum of {_maxLoanCount}"
                );
            }

            int remainingLoans = _maxLoanCount - customer.LoanCount;
            return ConditionResult.Success(
                null, // No eligible amount is determined by this condition
                $"Customer has {customer.LoanCount} active loans, below the maximum of {_maxLoanCount} (can take {remainingLoans} more)"
            );
        }
    }
}