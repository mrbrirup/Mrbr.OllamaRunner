using Mrbr.OllamaRunner.Models.Common;
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

    [JsonPropertyName("keep_alive")]
    public string? KeepAlive { get; set; }

    [JsonPropertyName("options")]
    public OllamaRuntimeOptions? Options { get; set; }
}