namespace Mrbr.OllamaRunner.Configuration;

/// <summary>
/// Configuration for a single Ollama server instance.
/// </summary>
public sealed class OllamaInstanceOptions {
    /// <summary>
    /// Logical instance name, for example "default", "coding", or "research".
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Whether this instance is externally managed or started by this runner.
    /// </summary>
    public OllamaInstanceMode Mode { get; set; } = OllamaInstanceMode.ManagedProcess;

    /// <summary>
    /// Host/IP address that the Ollama process should bind to.
    /// </summary>
    public string BindHost { get; set; } = "127.0.0.1";

    /// <summary>
    /// Port that the Ollama process should bind to.
    /// </summary>
    public int Port { get; set; } = 11434;

    /// <summary>
    /// Optional path for this instance's Ollama model store.
    /// Passed to the process as OLLAMA_MODELS.
    /// </summary>
    public string? ModelsPath { get; set; }

    /// <summary>
    /// Number of seconds to wait for Ollama to become available.
    /// </summary>
    public int StartupTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Additional environment variables passed to the Ollama process.
    /// </summary>
    public Dictionary<string, string> EnvironmentVariables { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Ollama API base address for this instance.
    /// </summary>
    public Uri ApiBaseUri => new($"http://{BindHost}:{Port}/api/");
}