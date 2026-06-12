namespace Mrbr.OllamaRunner.Configuration;

/// <summary>
/// Defines how an Ollama instance is managed by the runner.
/// </summary>
public enum OllamaInstanceMode {
    /// <summary>
    /// The runner does not start a process. It only connects to an existing Ollama server.
    /// </summary>
    External = 0,

    /// <summary>
    /// The runner starts and stops an ollama serve process.
    /// </summary>
    ManagedProcess = 1
}