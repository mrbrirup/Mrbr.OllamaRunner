namespace Mrbr.OllamaRunner.Configuration;

/// <summary>
/// Root configuration for the Ollama runner.
/// </summary>
public sealed class OllamaRunnerOptions {
    /// <summary>
    /// Full path to the Ollama executable.
    /// If omitted, the process runner will try to use "ollama" from PATH.
    /// </summary>
    public string? OllamaExecutablePath { get; set; }

    /// <summary>
    /// Named Ollama instances.
    /// </summary>
    public Dictionary<string, OllamaInstanceOptions> Instances { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}