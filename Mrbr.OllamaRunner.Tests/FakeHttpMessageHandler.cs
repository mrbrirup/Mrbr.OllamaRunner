namespace Mrbr.OllamaRunner.Tests;

internal sealed class FakeHttpMessageHandler : HttpMessageHandler {
    private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handler;

    public FakeHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler) {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken) {
        return Task.FromResult(_handler(request, cancellationToken));
    }
}