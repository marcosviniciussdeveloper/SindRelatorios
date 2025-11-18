using System.Text.Json.Serialization;

namespace SindRelatorios.Models.Entities;

public class Holiday
{
  [JsonPropertyName("date")]
  public string DataString { get; set; } = string.Empty;
  
  [JsonPropertyName("name")]
  public string Nome { get; set; } = string.Empty;
  
  public DateTime Data => DateTime.Parse(DataString);
  
}