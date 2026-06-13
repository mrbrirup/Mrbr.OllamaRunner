using Mrbr.OllamaRunner.Models.Chat;
using Mrbr.OllamaRunner.Models.Common;
using Mrbr.OllamaRunner.Models.Generate;

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

    IAsyncEnumerable<OllamaChatResponse> ChatStreamAsync(
        OllamaChatRequest request,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<OllamaChatResponse> ChatStreamAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options = null,
        string? keepAlive = null,
        CancellationToken cancellationToken = default);

    Task<OllamaGenerateResponse> GenerateAsync(
        OllamaGenerateRequest request,
        CancellationToken cancellationToken = default);

    Task<string> GenerateAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options = null,
        string? keepAlive = null,
        CancellationToken cancellationToken = default);
    IAsyncEnumerable<OllamaGenerateResponse> GenerateStreamAsync(
    OllamaGenerateRequest request,
    CancellationToken cancellationToken = default);

    IAsyncEnumerable<OllamaGenerateResponse> GenerateStreamAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options = null,
        string? keepAlive = null,
        CancellationToken cancellationToken = default);
}