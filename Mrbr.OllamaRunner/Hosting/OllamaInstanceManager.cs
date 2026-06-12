namespace Mrbr.OllamaRunner.Hosting;

/// <summary>
/// Default implementation of the Ollama instance manager.
/// </summary>
public sealed class OllamaInstanceManager : IOllamaInstanceManager {
    private readonly Dictionary<string, IOllamaInstance> _instances;

    public OllamaInstanceManager(IEnumerable<IOllamaInstance> instances) {
        _instances = instances.ToDictionary(
            instance => instance.Name,
            StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<IOllamaInstance> Instances => _instances.Values;

    public IOllamaInstance GetInstance(string name) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_instances.TryGetValue(name, out var instance))
            return instance;

        throw new KeyNotFoundException($"No Ollama instance named '{name}' is configured.");
    }

    public Task StartAsync(string name, CancellationToken cancellationToken = default) {
        return GetInstance(name).StartAsync(cancellationToken);
    }

    public async Task StartAllAsync(CancellationToken cancellationToken = default) {
        foreach (var instance in _instances.Values)
            await instance.StartAsync(cancellationToken);
    }

    public Task StopAsync(string name, CancellationToken cancellationToken = default) {
        return GetInstance(name).StopAsync(cancellationToken);
    }

    public async Task StopAllAsync(CancellationToken cancellationToken = default) {
        foreach (var instance in _instances.Values)
            await instance.StopAsync(cancellationToken);
    }
}