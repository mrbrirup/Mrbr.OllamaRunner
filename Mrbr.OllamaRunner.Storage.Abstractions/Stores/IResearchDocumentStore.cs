using Mrbr.OllamaRunner.Storage.Abstractions.Models;

namespace Mrbr.OllamaRunner.Storage.Abstractions.Stores;

/// <summary>
/// Stores and retrieves research documents.
/// </summary>
public interface IResearchDocumentStore {
    Task<ResearchDocument> ImportTextAsync(
        string projectId,
        string name,
        string text,
        string? sourceUri = null,
        string? originalFileName = null,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        ResearchDocument document,
        CancellationToken cancellationToken = default);

    Task<ResearchDocument?> GetAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default);

    Task<string?> GetExtractedTextAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResearchDocument>> ListAsync(
        string projectId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default);
}