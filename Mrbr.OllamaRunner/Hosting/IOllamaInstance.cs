namespace Mrbr.OllamaRunner.Hosting;

/// <summary>
/// Represents one configured Ollama server instance.
/// </summary>
public interface IOllamaInstance : IAsyncDisposable {
    /// <summary>
    /// Logical instance name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Base URI for the Ollama API.
    /// </summary>
    Uri ApiBaseUri { get; }

    /// <summary>
    /// True when this runner has a live managed process for the instance.
    /// For external instances, use WaitUntilReadyAsync to check availability.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Starts the instance, or verifies availability if externally managed.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the instance if it is managed by this runner.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits until the Ollama API responds.
    /// </summary>
    Task<bool> WaitUntilReadyAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Default chat model configured for this instance.
    /// </summary>
    string? DefaultModel { get; }
}