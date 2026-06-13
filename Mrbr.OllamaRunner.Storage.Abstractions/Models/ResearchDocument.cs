namespace Mrbr.OllamaRunner.Storage.Abstractions.Models;

/// <summary>
/// Represents a document imported into a research project.
/// </summary>
public sealed class ResearchDocument {
    /// <summary>
    /// Unique document id.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Project this document belongs to.
    /// </summary>
    public string ProjectId { get; set; } = default!;

    /// <summary>
    /// Display name or original file name.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Original file name, when imported from a file.
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// Source URI, when imported from a URL or crawler.
    /// </summary>
    public string? SourceUri { get; set; }

    /// <summary>
    /// Source type, for example Text, File, Url, News, Html, Pdf.
    /// </summary>
    public string SourceType { get; set; } = "Text";

    /// <summary>
    /// SHA-256 hash of the extracted content.
    /// </summary>
    public string ContentHash { get; set; } = default!;

    /// <summary>
    /// Relative path to the original imported content.
    /// </summary>
    public string OriginalPath { get; set; } = default!;

    /// <summary>
    /// Relative path to the extracted text content.
    /// </summary>
    public string ExtractedTextPath { get; set; } = default!;

    /// <summary>
    /// UTC import timestamp.
    /// </summary>
    public DateTimeOffset ImportedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// UTC update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Additional metadata for crawler, citation, extraction or indexing data.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = [];
}