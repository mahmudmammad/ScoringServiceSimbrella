# 🔧 Adding New Conditions to the Scoring System

> A comprehensive guide for extending the loan scoring system with custom conditions





## 🚀 Quick Start

### 1. Create Your Condition Class
The core design principle of the scoring service was modularity, ensuring that business condition(rules) were implemented as independent components. The Strategy Design Pattern
was used to encapsulate each condition in separate class that implement a shared interface
(ICondition "has evaluate method inside of it" ). This allowed conditions to be interchangeable and independently testable.
Each condition returns: 

• boolean indicating whether it is satisfied

• optional eligible amount

The Service iterates all condition results and determines whether the customer passes the
overall scoring based on predefined logic (you can enable or disable the condition if you want
in runtime).
```csharp
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
```


### 2. Register in Factory

```csharp
// In ConditionFactory.cs - CreateCondition method
   case "DefaultHistory":
                    bool failIfHasDefault = GetBoolParameter(config.Parameters, "FailIfHasDefault", true);
                    return new HasDefaultHistoryCondition(failIfHasDefault);   
```





###3: Database Configuration 

Add seed data for database-driven configuration:

```csharp
// In ScoringDbContext.OnModelCreating
  new ConditionConfigEntity
        {
            Id = 4,
            Type = "DefaultHistory",
            Enabled = false,
            Required = true,
            Priority = 4,
            ParametersJson = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "FailIfHasDefault", "true" }
            })
        }
```

###4: Add Migration

```csharp
dotnet ef migrations add <HasDefaultHistory>

```




