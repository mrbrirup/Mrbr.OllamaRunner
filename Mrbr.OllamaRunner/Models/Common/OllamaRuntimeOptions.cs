using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Common;

/// <summary>
/// Runtime model options sent to Ollama in the request "options" object.
/// </summary>
public sealed class OllamaRuntimeOptions {
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("num_ctx")]
    public int? NumCtx { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    [JsonPropertyName("repeat_penalty")]
    public double? RepeatPenalty { get; set; }

    [JsonPropertyName("seed")]
    public int? Seed { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object?> AdditionalOptions { get; set; } = [];
}