using Mrbr.OllamaRunner.Models.Common;
using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Embeddings;

/// <summary>
/// Request body for Ollama /api/embed.
/// </summary>
public sealed class OllamaEmbedRequest {
    [JsonPropertyName("model")]
    public string Model { get; set; } = default!;

    /// <summary>
    /// Text or array of texts to generate embeddings for.
    /// This is object because Ollama accepts either a single string or an array of strings.
    /// </summary>
    [JsonPropertyName("input")]
    public object Input { get; set; } = default!;

    [JsonPropertyName("truncate")]
    public bool? Truncate { get; set; }

    [JsonPropertyName("dimensions")]
    public int? Dimensions { get; set; }

    [JsonPropertyName("keep_alive")]
    public string? KeepAlive { get; set; }

    [JsonPropertyName("options")]
    public OllamaRuntimeOptions? Options { get; set; }
}