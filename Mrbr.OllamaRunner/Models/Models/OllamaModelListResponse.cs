using System.Text.Json.Serialization;

namespace Mrbr.OllamaRunner.Models.Models;

/// <summary>
/// Response body from Ollama GET /api/tags.
/// </summary>
public sealed class OllamaModelListResponse {
    [JsonPropertyName("models")]
    public List<OllamaModelInfo> Models { get; set; } = [];
    public bool ContainsModel(string modelName) {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        return Models.Any(model =>
            string.Equals(model.Name, modelName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(model.Model, modelName, StringComparison.OrdinalIgnoreCase));
    }
}