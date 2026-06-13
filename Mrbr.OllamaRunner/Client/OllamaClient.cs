using Mrbr.OllamaRunner.Models.Chat;
using Mrbr.OllamaRunner.Models.Common;

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

    public Task<string> ChatAsync(string model, string prompt, CancellationToken cancellationToken = default) {
        return ChatAsync(
            model,
            prompt,
            options: null,
            keepAlive: null,
            cancellationToken);
    }

    // TODO: Consider removing the commented-out method below if it's no longer needed, as it duplicates functionality with the new method that includes options and keepAlive parameters.
    //public async Task<string> ChatAsync(string model, string prompt, CancellationToken cancellationToken = default) {
    //    ArgumentException.ThrowIfNullOrWhiteSpace(model);
    //    ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

    //    var response = await ChatAsync(
    //        new OllamaChatRequest {
    //            Model = model,
    //            Messages =
    //            [
    //                new OllamaChatMessage
    //                {
    //                    Role = "user",
    //                    Content = prompt
    //                }
    //            ],
    //            Stream = false
    //        },
    //        cancellationToken);

    //    return response.Message?.Content
    //        ?? throw new InvalidOperationException("Ollama returned no message content.");
    //}


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
}