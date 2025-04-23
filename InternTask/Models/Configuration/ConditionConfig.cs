namespace InternTask.Models.Configuration;

public class ConditionConfig
{
    /// <summary>
    /// Type of the condition
    /// </summary>
    public string Type { get; set; }
        
    /// <summary>
    /// Whether the condition is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;
        
    /// <summary>
    /// Whether the condition is required for approval
    /// </summary>
    public bool Required { get; set; }
        
    /// <summary>
    /// Priority of the condition (lower numbers = higher priority)
    /// </summary>
    public int Priority { get; set; }
        
    /// <summary>
    /// Parameters specific to this condition type
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
}