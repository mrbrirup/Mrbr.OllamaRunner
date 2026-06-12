using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Chat;

/// <summary>
/// Request body for Ollama /api/chat.
/// </summary>
public sealed class OllamaChatRequest {
    [JsonPropertyName("model")]
    public string Model { get; set; } = default!;

    [JsonPropertyName("messages")]
    public List<OllamaChatMessage> Messages { get; set; } = [];

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}