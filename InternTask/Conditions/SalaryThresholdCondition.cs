using InternTask.Interfaces;
using InternTask.Models;

namespace InternTask.Conditions;

public class SalaryThresholdCondition : ICondition
{
    private readonly decimal _minimumSalary;
    private readonly decimal _salaryMultiplier;

    public string Id => "SalaryThreshold";
    public string Name => "Minimum Salary Requirement";
    public int Priority => 1; 
    public bool IsRequired => true;

    public SalaryThresholdCondition(decimal minimumSalary = 30000, decimal salaryMultiplier = 3.0m)
    {
        _minimumSalary = minimumSalary;
        _salaryMultiplier = salaryMultiplier;
    }

    public ConditionResult Evaluate(Customer customer)
    {
        if (customer == null)
        {
            return ConditionResult.Failure("Customer information is missing");
        }

        if (customer.Salary < _minimumSalary)
        {
            return ConditionResult.Failure($"Salary ({customer.Salary:C}) is below the minimum requirement of {_minimumSalary:C}");
        }


        decimal eligibleAmount = customer.Salary * _salaryMultiplier;
            
        return ConditionResult.Success(
            eligibleAmount,
            $"Customer salary of {customer.Salary:C} qualifies for a loan up to {eligibleAmount:C}"
        );
    }
}