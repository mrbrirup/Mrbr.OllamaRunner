using Mrbr.OllamaRunner.Hosting;

namespace Mrbr.OllamaRunner.Client;

/// <summary>
/// Default factory for creating Ollama API clients.
/// </summary>
public sealed class OllamaClientFactory : IOllamaClientFactory {
    private readonly IOllamaInstanceManager _instanceManager;

    public OllamaClientFactory(IOllamaInstanceManager instanceManager) {
        _instanceManager = instanceManager;
    }

    public IOllamaClient CreateClient(string instanceName) {
        var instance = _instanceManager.GetInstance(instanceName);

        var httpClient = new HttpClient {
            BaseAddress = instance.ApiBaseUri,
            Timeout = Timeout.InfiniteTimeSpan
        };

        return new OllamaClient(httpClient);
    }
}