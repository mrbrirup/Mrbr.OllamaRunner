using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Chat;

/// <summary>
/// A single Ollama chat message.
/// </summary>
public sealed class OllamaChatMessage {
    [JsonPropertyName("role")]
    public string Role { get; set; } = default!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;
}