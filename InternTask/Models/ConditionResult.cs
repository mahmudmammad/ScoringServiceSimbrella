namespace InternTask.Models;

public class ConditionResult
{
    public bool Passed { get; set; }


    public decimal? EligibleAmount { get; set; }

    public string Message { get; set; }

    public static ConditionResult Success (decimal? eligibleAmount = null, string message = null)
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
            EligibleAmount = null,
            Message = message
        };
    }

}