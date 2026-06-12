using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Chat;

/// <summary>
/// Non-streaming response body from Ollama /api/chat.
/// </summary>
public sealed class OllamaChatResponse {
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("message")]
    public OllamaChatMessage? Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}