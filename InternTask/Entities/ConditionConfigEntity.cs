using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace InternTask.Entities;

[Table("ConditionConfigs")]
public class ConditionConfigEntity
{
    [Key]
    public int Id { get; set; }

    public string Type { get; set; }

    public bool Enabled { get; set; } = true;

    public bool Required { get; set; }

    public int Priority { get; set; }

    // Store the dictionary as a JSON string
    public string ParametersJson
    {
        get => JsonSerializer.Serialize(Parameters);
        set => Parameters = string.IsNullOrWhiteSpace(value)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(value);
    }

    [NotMapped]
    public Dictionary<string, string> Parameters { get; set; } = new();
}