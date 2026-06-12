namespace Mrbr.OllamaRunner.Hosting;

/// <summary>
/// Manages configured Ollama instances.
/// </summary>
public interface IOllamaInstanceManager {
    IReadOnlyCollection<IOllamaInstance> Instances { get; }

    IOllamaInstance GetInstance(string name);

    Task StartAsync(string name, CancellationToken cancellationToken = default);

    Task StartAllAsync(CancellationToken cancellationToken = default);

    Task StopAsync(string name, CancellationToken cancellationToken = default);

    Task StopAllAsync(CancellationToken cancellationToken = default);
}