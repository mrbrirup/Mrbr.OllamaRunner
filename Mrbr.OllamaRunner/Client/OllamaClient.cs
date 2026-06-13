using Mrbr.OllamaRunner.Models.Chat;
using Mrbr.OllamaRunner.Models.Common;
using Mrbr.OllamaRunner.Models.Generate;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Mrbr.OllamaRunner.Client;

/// <summary>
/// Default Ollama HTTP API client.
/// </summary>
public sealed class OllamaClient : IOllamaClient {
    private readonly HttpClient _httpClient;

    public OllamaClient(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public async Task<OllamaChatResponse> ChatAsync(
        OllamaChatRequest request,
        CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request);

        request.Stream = false;

        using var response = await _httpClient.PostAsJsonAsync(
            "chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OllamaChatResponse>(
            cancellationToken)
            ?? throw new InvalidOperationException("Ollama returned an empty chat response.");
    }

    public Task<string> ChatAsync(
        string model,
        string prompt,
        CancellationToken cancellationToken = default) {
        return ChatAsync(
            model,
            prompt,
            options: null,
            keepAlive: null,
            cancellationToken);
    }

    public async Task<string> ChatAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options,
        string? keepAlive = null,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var response = await ChatAsync(
            new OllamaChatRequest {
                Model = model,
                Messages =
                [
                    new OllamaChatMessage
                    {
                        Role = "user",
                        Content = prompt
                    }
                ],
                Stream = false,
                KeepAlive = keepAlive,
                Options = options
            },
            cancellationToken);

        return response.Message?.Content
            ?? throw new InvalidOperationException("Ollama returned no message content.");
    }

    public async IAsyncEnumerable<OllamaChatResponse> ChatStreamAsync(
        OllamaChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request);

        request.Stream = true;

        using var response = await _httpClient.PostAsJsonAsync(
            "chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream) {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line);

            if (chunk is not null)
                yield return chunk;
        }
    }

    public IAsyncEnumerable<OllamaChatResponse> ChatStreamAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options = null,
        string? keepAlive = null,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        return ChatStreamAsync(
            new OllamaChatRequest {
                Model = model,
                Messages =
                [
                    new OllamaChatMessage
                    {
                        Role = "user",
                        Content = prompt
                    }
                ],
                Stream = true,
                KeepAlive = keepAlive,
                Options = options
            },
            cancellationToken);
    }
    public async Task<OllamaGenerateResponse> GenerateAsync(
    OllamaGenerateRequest request,
    CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request);

        request.Stream = false;

        using var response = await _httpClient.PostAsJsonAsync(
            "generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>(
            cancellationToken)
            ?? throw new InvalidOperationException("Ollama returned an empty generate response.");
    }

    public async Task<string> GenerateAsync(
        string model,
        string prompt,
        OllamaRuntimeOptions? options = null,
        string? keepAlive = null,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

        var response = await GenerateAsync(
            new OllamaGenerateRequest {
                Model = model,
                Prompt = prompt,
                Stream = false,
                KeepAlive = keepAlive,
                Options = options
            },
            cancellationToken);

        return response.Response
            ?? throw new InvalidOperationException("Ollama returned no generated response content.");
    }
}