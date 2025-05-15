using InternTask.Interfaces;
using InternTask.Models;

namespace InternTask.Conditions
{
    public class HasDefaultHistoryCondition : ICondition
    {
        private readonly bool _failIfHasDefault;

        public string Id => "DefaultHistory";
        public string Name => "Default History Check";
        public int Priority => 1;
        public bool IsRequired => true; // Defaults are a serious risk, make this required

        public HasDefaultHistoryCondition(bool failIfHasDefault = true)
        {
            _failIfHasDefault = failIfHasDefault;
        }

        public ConditionResult Evaluate(Customer customer)
        {
            if (customer == null)
            {
                return ConditionResult.Failure("Customer information is missing");
            }

            if (_failIfHasDefault && customer.HasDefaultHistory)
            {
                return ConditionResult.Failure("Customer has a history of loan defaults");
            }

            return ConditionResult.Success(0, "Customer has no history of loan defaults");
        }

        public Dictionary<string, object> GetConfiguration()
        {
            return new Dictionary<string, object>
            {
                { "failIfHasDefault", _failIfHasDefault }
            };
        }
    }
}