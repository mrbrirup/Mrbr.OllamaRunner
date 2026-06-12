namespace Mrbr.OllamaRunner.Client;

/// <summary>
/// Creates Ollama API clients for named instances.
/// </summary>
public interface IOllamaClientFactory {
    IOllamaClient CreateClient(string instanceName);
}