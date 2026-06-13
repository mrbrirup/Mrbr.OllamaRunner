namespace Mrbr.OllamaRunner.Storage.Abstractions.Models;

/// <summary>
/// Represents a local research project.
/// </summary>
public sealed class ResearchProject {
    /// <summary>
    /// Unique project id.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Display name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// UTC creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// UTC update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Additional metadata for later use.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];
}