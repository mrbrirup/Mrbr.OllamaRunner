using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.Models.Chat;
using Mrbr.OllamaRunner.Models.Common;
using System.Net;
using System.Text;
using Xunit;

namespace Mrbr.OllamaRunner.Tests;

public sealed class OllamaClientTests {
    [Fact]
    public async Task ChatAsync_SendsRequestToChatEndpoint() {
        HttpRequestMessage? capturedRequest = null;

        var handler = new FakeHttpMessageHandler((request, _) => {
            capturedRequest = request;

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(
                    """
                    {
                      "model": "llama3.2:1b",
                      "created_at": "2026-06-12T12:00:00Z",
                      "message": {
                        "role": "assistant",
                        "content": "Hello"
                      },
                      "done": true
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        var response = await client.ChatAsync(new OllamaChatRequest {
            Model = "llama3.2:1b",
            Messages =
            [
                new OllamaChatMessage
                {
                    Role = "user",
                    Content = "Say hello."
                }
            ]
        });

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal(
            new Uri("http://127.0.0.1:11434/api/chat"),
            capturedRequest.RequestUri);

        Assert.Equal("Hello", response.Message?.Content);
        Assert.True(response.Done);
    }

    [Fact]
    public async Task ChatAsync_StringOverload_ReturnsMessageContent() {
        var handler = new FakeHttpMessageHandler((_, _) =>
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(
                    """
                    {
                      "model": "llama3.2:1b",
                      "message": {
                        "role": "assistant",
                        "content": "Hello"
                      },
                      "done": true
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            });

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        var result = await client.ChatAsync(
            "llama3.2:1b",
            "Say hello.");

        Assert.Equal("Hello", result);
    }

    [Fact]
    public async Task ChatAsync_WhenHttpFails_ThrowsHttpRequestException() {
        var handler = new FakeHttpMessageHandler((_, _) =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            client.ChatAsync(new OllamaChatRequest {
                Model = "llama3.2:1b",
                Messages =
                [
                    new OllamaChatMessage
                    {
                        Role = "user",
                        Content = "Hello"
                    }
                ]
            }));
    }
    [Fact]
    public async Task ChatStreamAsync_ReadsNewlineDelimitedChunks() {
        var handler = new FakeHttpMessageHandler((_, _) =>
            new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(
                    """
                {"model":"llama3.2:1b","message":{"role":"assistant","content":"Hel"},"done":false}
                {"model":"llama3.2:1b","message":{"role":"assistant","content":"lo"},"done":false}
                {"model":"llama3.2:1b","message":{"role":"assistant","content":""},"done":true}
                """,
                    Encoding.UTF8,
                    "application/x-ndjson")
            });

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        var chunks = new List<OllamaChatResponse>();

        await foreach (var chunk in client.ChatStreamAsync(new OllamaChatRequest {
            Model = "llama3.2:1b",
            Messages =
            [
                new OllamaChatMessage
            {
                Role = "user",
                Content = "Say hello."
            }
            ]
        })) {
            chunks.Add(chunk);
        }

        Assert.Equal(3, chunks.Count);
        Assert.Equal("Hel", chunks[0].Message?.Content);
        Assert.Equal("lo", chunks[1].Message?.Content);
        Assert.True(chunks[2].Done);
    }
    [Fact]
    public async Task ChatStreamAsync_WithOptions_SendsOptionsAndKeepAlive() {
        string? requestJson = null;

        var handler = new FakeHttpMessageHandler((request, _) => {
            requestJson = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(
                    """
                {"model":"llama3.2:1b","message":{"role":"assistant","content":"Hello"},"done":false}
                {"model":"llama3.2:1b","message":{"role":"assistant","content":""},"done":true}
                """,
                    Encoding.UTF8,
                    "application/x-ndjson")
            };
        });

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        var chunks = new List<OllamaChatResponse>();

        await foreach (var chunk in client.ChatStreamAsync(
            "llama3.2:1b",
            "Say hello.",
            new OllamaRuntimeOptions {
                Temperature = 0.2,
                NumCtx = 2048
            },
            keepAlive: "5m")) {
            chunks.Add(chunk);
        }

        Assert.NotNull(requestJson);
        Assert.Contains("\"stream\":true", requestJson);
        Assert.Contains("\"keep_alive\":\"5m\"", requestJson);
        Assert.Contains("\"temperature\":0.2", requestJson);
        Assert.Contains("\"num_ctx\":2048", requestJson);

        Assert.Equal(2, chunks.Count);
        Assert.Equal("Hello", chunks[0].Message?.Content);
        Assert.True(chunks[^1].Done);
    }
    [Fact]
    public async Task ChatStreamAsync_WhenHttpFails_ThrowsHttpRequestException() {
        var handler = new FakeHttpMessageHandler((_, _) =>
            new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(async () => {
            await foreach (var _ in client.ChatStreamAsync(new OllamaChatRequest {
                Model = "llama3.2:1b",
                Messages =
                [
                    new OllamaChatMessage
                {
                    Role = "user",
                    Content = "Hello"
                }
                ]
            })) {
            }
        });
    }
}