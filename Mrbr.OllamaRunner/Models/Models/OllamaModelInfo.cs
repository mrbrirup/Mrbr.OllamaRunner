using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Models;

/// <summary>
/// Describes a locally available Ollama model.
/// </summary>
public sealed class OllamaModelInfo {
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("modified_at")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [JsonPropertyName("size")]
    public long? Size { get; set; }

    [JsonPropertyName("digest")]
    public string? Digest { get; set; }

    [JsonPropertyName("details")]
    public OllamaModelDetails? Details { get; set; }
}