using InternTask.Interfaces;
using InternTask.Models;

namespace InternTask.Conditions
{
 
    public class AccountBalanceCondition : ICondition
    {
        private readonly decimal _minimumBalance;
        private readonly decimal _balanceMultiplier;

        public string Id => "AccountBalance";
        public string Name => "Minimum Account Balance Check";
        public int Priority => 3;
        public bool IsRequired => false;

        public AccountBalanceCondition(decimal minimumBalance = 1000, decimal balanceMultiplier = 2.0m)
        {
            _minimumBalance = minimumBalance;
            _balanceMultiplier = balanceMultiplier;
        }

        public ConditionResult Evaluate(Customer customer)
        {
            if (customer == null)
            {
                return ConditionResult.Failure("Customer information is missing");
            }

            if (customer.AccountBalance < _minimumBalance)
            {
                return ConditionResult.Failure(
                    $"Account balance ({customer.AccountBalance:C}) is below the minimum requirement of {_minimumBalance:C}"
                );
            }

            decimal eligibleAmountFromBalance = customer.AccountBalance * _balanceMultiplier;
            
            return ConditionResult.Success(
                eligibleAmountFromBalance,
                $"Account balance of {customer.AccountBalance:C} allows for additional {eligibleAmountFromBalance:C} in loan amount"
            );
        }
        public Dictionary<string, object> GetConfiguration()
        {
            return new Dictionary<string, object>
            {
                { "minimumBalance", _minimumBalance },
                { "balanceMultiplier", _balanceMultiplier }
            };
        }
    }
}