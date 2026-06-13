using Mrbr.OllamaRunner.Models.Common;
using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Generate;

/// <summary>
/// Request body for Ollama /api/generate.
/// </summary>
public sealed class OllamaGenerateRequest {
    [JsonPropertyName("model")]
    public string Model { get; set; } = default!;

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("system")]
    public string? System { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;

    [JsonPropertyName("keep_alive")]
    public string? KeepAlive { get; set; }

    [JsonPropertyName("options")]
    public OllamaRuntimeOptions? Options { get; set; }

    [JsonPropertyName("raw")]
    public bool? Raw { get; set; }

    [JsonPropertyName("format")]
    public object? Format { get; set; }
}