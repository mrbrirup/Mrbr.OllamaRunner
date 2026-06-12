using Mrbr.OllamaRunner.Models.Chat;

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

    public async Task<string> ChatAsync(
        string model,
        string prompt,
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
                Stream = false
            },
            cancellationToken);

        return response.Message?.Content
            ?? throw new InvalidOperationException("Ollama returned no message content.");
    }
}