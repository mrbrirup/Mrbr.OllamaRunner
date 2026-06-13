using Mrbr.OllamaRunner.Models.Chat;
using Mrbr.OllamaRunner.Models.Common;

namespace Mrbr.OllamaRunner.Client;

/// <summary>
/// Client for the Ollama HTTP API.
/// </summary>
public interface IOllamaClient {
    Task<OllamaChatResponse> ChatAsync(
        OllamaChatRequest request,
        CancellationToken cancellationToken = default);

    Task<string> ChatAsync(
        string model,
        string prompt,
        CancellationToken cancellationToken = default);

    Task<string> ChatAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options,
        string? keepAlive = null,
        CancellationToken cancellationToken = default);
}