using Mrbr.OllamaRunner.Storage.Abstractions.Models;

namespace Mrbr.OllamaRunner.Storage.Abstractions.Stores;

/// <summary>
/// Stores and retrieves research projects.
/// </summary>
public interface IResearchProjectStore {
    Task<ResearchProject> CreateAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        ResearchProject project,
        CancellationToken cancellationToken = default);

    Task<ResearchProject?> GetAsync(
        string projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ResearchProject>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string projectId,
        CancellationToken cancellationToken = default);
}