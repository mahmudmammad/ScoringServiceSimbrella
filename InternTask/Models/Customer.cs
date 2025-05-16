using System.Text.Json.Serialization;

namespace InternTask.Models;

public class Customer
{
    [JsonIgnore]
    public int Id { get; set; }

    public decimal Salary { get; set; }

    public int LoanCount { get; set; }

    public decimal AccountBalance { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; }

    public bool HasDefaultHistory{ get; set; }
    
}