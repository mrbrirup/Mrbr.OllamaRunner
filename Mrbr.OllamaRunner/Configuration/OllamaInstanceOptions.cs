using Mrbr.OllamaRunner.Models.Common;

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

    /// <summary>
    /// Default chat model used by this instance when no model is supplied explicitly.
    /// </summary>
    public string? DefaultModel { get; set; }

    /// <summary>
    /// Optional default keep-alive value used by higher-level request helpers.
    /// Examples: "5m", "30m", "1h", "0".
    /// </summary>
    public string? DefaultKeepAlive { get; set; }

    /// <summary>
    /// Optional default runtime options used by higher-level request helpers.
    /// </summary>
    public OllamaRuntimeOptions? DefaultRuntimeOptions { get; set; }
    /// <summary>
    /// Default embedding model used by this instance when no embedding model is supplied explicitly.
    /// </summary>
    public string? DefaultEmbeddingModel { get; set; }

    /// <summary>
    /// Optional default embedding dimensions.
    /// Some embedding models/endpoints support reducing output dimensions.
    /// </summary>
    public int? DefaultEmbeddingDimensions { get; set; }

    /// <summary>
    /// Whether embedding inputs should be truncated when they exceed the model context window.
    /// Ollama defaults this to true.
    /// </summary>
    public bool? DefaultEmbeddingTruncate { get; set; }
}