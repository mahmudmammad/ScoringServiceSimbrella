# Scoring Service

A .NET-based Web API that evaluates customer loan eligibility using configurable business conditions. The project leverages PostgreSQL for data persistence, Prometheus for monitoring, and Docker for containerized deployment.

## 🛠 Features

- **Dynamic Rule Engine**: Customer scoring based on configurable business conditions
- **Monitoring Integration**: Prometheus metrics with Grafana dashboards
- **Database Support**: PostgreSQL for reliable data persistence
- **Containerized Deployment**: Docker-ready for easy setup and scaling
- **Modular Architecture**: Strategy pattern implementation for extensible conditions

## 🚀 Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET 8.0 SDK (for development)


### Quick Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/mahmudmammad/ScoringServiceSimbrella
   cd InternTask
   ```

2. **Build and run with Docker**
   ```bash
   docker-compose build
   docker-compose up
   ```

### Access Points

Once running, the following services will be available:

| Service | URL | Description |
|---------|-----|-------------|
| **API** | http://localhost:8080 | Main scoring service endpoint |
| **Prometheus** | http://localhost:9090 | Metrics collection and monitoring |
| **Grafana** | http://localhost:3000 | Metrics visualization dashboard |

## 🏗 Architecture

The scoring system is built on a modular architecture using the **Strategy Design Pattern**. Each business condition is implemented as an independent component that implements the `ICondition` interface, ensuring:

- **Modularity**: Conditions are interchangeable and independently testable
- **Flexibility**: Easy to add, remove, or modify conditions at runtime
- **Maintainability**: Each condition is self-contained with clear responsibilities

### Core Components

- **ICondition Interface**: Defines the contract for all scoring conditions
- **ConditionFactory**: Creates and configures condition instances
- **ScoringService**: Orchestrates condition evaluation and final scoring
- **Database Layer**: Stores condition configurations

## 🔧 Adding New Conditions

Follow these steps to extend the scoring system with custom business rules:

### Step 1: Create Your Condition Class

Implement the `ICondition` interface in a new class:

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
        public bool IsRequired => true; // Critical risk factor
        
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

### Step 2: Register in Factory

Add your condition to the `ConditionFactory.cs`:

```csharp
// In ConditionFactory.cs - CreateCondition method
case "DefaultHistory":
    bool failIfHasDefault = GetBoolParameter(config.Parameters, "FailIfHasDefault", true);
    return new HasDefaultHistoryCondition(failIfHasDefault);
```

### Step 3:  Configuration

Add seed data for your new condition:

```csharp
 "ScoringService": {
    "Conditions": [
      {
        "Type": "SalaryThreshold",
        "Enabled": true,
        "Required": true,
        "Priority": 1,
        "Parameters": {
          "MinimumSalary": "5000",
          "SalaryMultiplier": "1.2"
        }
      },
      {
        "Type": "LoanCountLimit",
        "Enabled": true,
        "Required": true,
        "Priority": 2,
        "Parameters": {
          "MaxLoanCount": "5"
        }
      },
      {
        "Type": "AccountBalance",
        "Enabled": true,
        "Required": false,
        "Priority": 3,
        "Parameters": {
          "MinimumBalance": "2000",
          "BalanceMultiplier": "2.0"
        }
      }
    ,

      {
        "Type": "DefaultHistory",
        "Enabled": false,
        "Required": true,
        "Priority": 4,
        "Parameters": {
          "FailIfHasDefault": "true"
        }}
    ]
  }
```

### Step 4: Apply Database Migration

Create and apply the migration:

```bash
dotnet ef migrations add AddHasDefaultHistoryCondition
dotnet ef database update
```

## 📊 Condition Evaluation Flow

Each condition returns a `ConditionResult` containing:

- **Success/Failure Status**: Boolean indicating if the condition is satisfied
- **Eligible Amount**: Optional amount for conditions that affect loan limits
- **Message**: Descriptive feedback for logging and debugging

The scoring service processes all enabled conditions and determines overall eligibility based on:

- Required conditions must all pass
- Optional conditions contribute to the final score
- Priority determines evaluation order

## 🔍 Monitoring and Observability

The service includes comprehensive monitoring through:

- **Prometheus Metrics**: Request counts, response times, condition evaluation stats
- **Grafana Dashboards**: Visual representation of system health and performance
- **Structured Logging**: Detailed logs for debugging and audit trails

## 🧪 Testing

The modular design facilitates comprehensive testing:

- **Unit Tests**: Each condition can be tested in isolation
- **Performance Tests**: Load testing with various condition combinations

## 📈 Performance Considerations

- Conditions are evaluated in priority order for optimal performance





