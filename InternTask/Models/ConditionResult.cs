namespace InternTask.Models;

public class ConditionResult
{
    public bool Passed { get; set; }


    public decimal EligibleAmount { get; set; } = 0m;

    public string Message { get; set; }

    public static ConditionResult Success (decimal eligibleAmount , string message = null)
    {

        return new ConditionResult
        {
            Passed = true,
            EligibleAmount = eligibleAmount,
            Message = message
        };
    }
    
    public static ConditionResult Failure(string message = null)
    {
        return new ConditionResult
        {
            Passed = false,
            EligibleAmount = 0,
            Message = message
        };
    }

}