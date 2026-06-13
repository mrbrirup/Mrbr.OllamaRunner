using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Storage.Abstractions.Models;
using Mrbr.OllamaRunner.Storage.Abstractions.Stores;
using Mrbr.OllamaRunner.Storage.FileSystem.Internal;
using System.Text.Json;

namespace Mrbr.OllamaRunner.Storage.FileSystem.Stores;

/// <summary>
/// File-system implementation of research project storage.
/// </summary>
public sealed class FileSystemResearchProjectStore : IResearchProjectStore {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true
    };

    private readonly ResearchStoragePathProvider _pathProvider;
    private readonly ILogger<FileSystemResearchProjectStore> _logger;

    internal FileSystemResearchProjectStore(
        ResearchStoragePathProvider pathProvider,
        ILogger<FileSystemResearchProjectStore> logger) {
        _pathProvider = pathProvider;
        _logger = logger;
    }

    public async Task<ResearchProject> CreateAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var project = new ResearchProject {
            Name = name,
            Description = description,
            CreatedUtc = DateTimeOffset.UtcNow,
            UpdatedUtc = DateTimeOffset.UtcNow
        };

        await SaveAsync(project, cancellationToken);

        return project;
    }

    public async Task SaveAsync(
        ResearchProject project,
        CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentException.ThrowIfNullOrWhiteSpace(project.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(project.Name);

        _pathProvider.EnsureRootCreated();
        _pathProvider.EnsureProjectCreated(project.Id);

        project.UpdatedUtc = DateTimeOffset.UtcNow;

        var projectJsonPath = _pathProvider.GetProjectJsonPath(project.Id);

        await using var stream = File.Create(projectJsonPath);

        await JsonSerializer.SerializeAsync(
            stream,
            project,
            JsonOptions,
            cancellationToken);

        _logger.LogInformation(
            "Saved research project {ProjectId} to {ProjectJsonPath}",
            project.Id,
            projectJsonPath);
    }

    public async Task<ResearchProject?> GetAsync(
        string projectId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        var projectPath = _pathProvider.GetProjectPath(projectId);

        if (!Directory.Exists(projectPath))
            return null;

        var projectJsonPath = _pathProvider.GetProjectJsonPath(projectId);

        if (!File.Exists(projectJsonPath))
            return null;

        await using var stream = File.OpenRead(projectJsonPath);

        return await JsonSerializer.DeserializeAsync<ResearchProject>(
            stream,
            JsonOptions,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ResearchProject>> ListAsync(
        CancellationToken cancellationToken = default) {
        _pathProvider.EnsureRootCreated();

        if (!Directory.Exists(_pathProvider.ProjectsPath))
            return [];

        var projects = new List<ResearchProject>();

        foreach (var projectDirectory in Directory.EnumerateDirectories(_pathProvider.ProjectsPath)) {
            cancellationToken.ThrowIfCancellationRequested();

            var projectJsonPath = Path.Combine(projectDirectory, "project.json");

            if (!File.Exists(projectJsonPath))
                continue;

            await using var stream = File.OpenRead(projectJsonPath);

            var project = await JsonSerializer.DeserializeAsync<ResearchProject>(
                stream,
                JsonOptions,
                cancellationToken);

            if (project is not null)
                projects.Add(project);
        }

        return projects
            .OrderBy(project => project.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public Task<bool> ExistsAsync(
        string projectId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        var projectPath = _pathProvider.GetProjectPath(projectId);

        if (!Directory.Exists(projectPath))
            return Task.FromResult(false);

        var projectJsonPath = _pathProvider.GetProjectJsonPath(projectId);

        return Task.FromResult(File.Exists(projectJsonPath));
    }
}