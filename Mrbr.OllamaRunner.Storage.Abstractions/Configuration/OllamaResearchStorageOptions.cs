namespace Mrbr.OllamaRunner.Storage.Abstractions.Configuration;

/// <summary>
/// Storage configuration for local research data.
/// </summary>
public sealed class OllamaResearchStorageOptions {
    /// <summary>
    /// Root folder for all research data.
    /// </summary>
    public string RootPath { get; set; } = default!;

    /// <summary>
    /// Whether required directories should be created automatically.
    /// </summary>
    public bool CreateDirectories { get; set; } = true;
}