using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Storage.Abstractions.Models;
using Mrbr.OllamaRunner.Storage.Abstractions.Stores;
using Mrbr.OllamaRunner.Storage.FileSystem.Internal;
using System.Text.Json;

namespace Mrbr.OllamaRunner.Storage.FileSystem.Stores;

/// <summary>
/// File-system implementation of research document storage.
/// </summary>
public sealed class FileSystemResearchDocumentStore : IResearchDocumentStore {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        WriteIndented = true
    };

    private readonly ResearchStoragePathProvider _pathProvider;
    private readonly ILogger<FileSystemResearchDocumentStore> _logger;

    internal FileSystemResearchDocumentStore(
        ResearchStoragePathProvider pathProvider,
        ILogger<FileSystemResearchDocumentStore> logger) {
        _pathProvider = pathProvider;
        _logger = logger;
    }

    public async Task<ResearchDocument> ImportTextAsync(
        string projectId,
        string name,
        string text,
        string? sourceUri = null,
        string? originalFileName = null,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var document = new ResearchDocument {
            ProjectId = projectId,
            Name = name,
            OriginalFileName = originalFileName,
            SourceUri = sourceUri,
            SourceType = sourceUri is null ? "Text" : "Url",
            ContentHash = ContentHash.Sha256Hex(text),
            ImportedUtc = DateTimeOffset.UtcNow,
            UpdatedUtc = DateTimeOffset.UtcNow
        };

        var originalPath = _pathProvider.GetOriginalTextPath(projectId, document.Id);
        var extractedTextPath = _pathProvider.GetExtractedTextPath(projectId, document.Id);

        document.OriginalPath = GetProjectRelativePath(projectId, originalPath);
        document.ExtractedTextPath = GetProjectRelativePath(projectId, extractedTextPath);

        _pathProvider.EnsureRootCreated();
        _pathProvider.EnsureProjectCreated(projectId);
        _pathProvider.EnsureDocumentsCreated(projectId);
        _pathProvider.EnsureDocumentCreated(projectId, document.Id);

        await File.WriteAllTextAsync(
            originalPath,
            text,
            cancellationToken);

        await File.WriteAllTextAsync(
            extractedTextPath,
            text,
            cancellationToken);

        await SaveAsync(document, cancellationToken);

        _logger.LogInformation(
            "Imported research document {DocumentId} into project {ProjectId}",
            document.Id,
            projectId);

        return document;
    }

    public async Task SaveAsync(
        ResearchDocument document,
        CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(document.ProjectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(document.Id);
        ArgumentException.ThrowIfNullOrWhiteSpace(document.Name);

        _pathProvider.EnsureRootCreated();
        _pathProvider.EnsureProjectCreated(document.ProjectId);
        _pathProvider.EnsureDocumentsCreated(document.ProjectId);
        _pathProvider.EnsureDocumentCreated(document.ProjectId, document.Id);

        document.UpdatedUtc = DateTimeOffset.UtcNow;

        var documentJsonPath = _pathProvider.GetDocumentJsonPath(
            document.ProjectId,
            document.Id);

        await using var stream = File.Create(documentJsonPath);

        await JsonSerializer.SerializeAsync(
            stream,
            document,
            JsonOptions,
            cancellationToken);
    }

    public async Task<ResearchDocument?> GetAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);

        var documentPath = _pathProvider.GetDocumentPath(projectId, documentId);

        if (!Directory.Exists(documentPath))
            return null;

        var documentJsonPath = _pathProvider.GetDocumentJsonPath(projectId, documentId);

        if (!File.Exists(documentJsonPath))
            return null;

        await using var stream = File.OpenRead(documentJsonPath);

        return await JsonSerializer.DeserializeAsync<ResearchDocument>(
            stream,
            JsonOptions,
            cancellationToken);
    }

    public async Task<string?> GetExtractedTextAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);

        var extractedTextPath = _pathProvider.GetExtractedTextPath(projectId, documentId);

        if (!File.Exists(extractedTextPath))
            return null;

        return await File.ReadAllTextAsync(
            extractedTextPath,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ResearchDocument>> ListAsync(
        string projectId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        _pathProvider.EnsureRootCreated();
        _pathProvider.EnsureProjectCreated(projectId);
        _pathProvider.EnsureDocumentsCreated(projectId);

        var documentsPath = _pathProvider.GetDocumentsPath(projectId);

        if (!Directory.Exists(documentsPath))
            return [];

        var documents = new List<ResearchDocument>();

        foreach (var documentDirectory in Directory.EnumerateDirectories(documentsPath)) {
            cancellationToken.ThrowIfCancellationRequested();

            var documentJsonPath = Path.Combine(documentDirectory, "document.json");

            if (!File.Exists(documentJsonPath))
                continue;

            await using var stream = File.OpenRead(documentJsonPath);

            var document = await JsonSerializer.DeserializeAsync<ResearchDocument>(
                stream,
                JsonOptions,
                cancellationToken);

            if (document is not null)
                documents.Add(document);
        }

        return documents
            .OrderBy(document => document.Name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public Task<bool> ExistsAsync(
        string projectId,
        string documentId,
        CancellationToken cancellationToken = default) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);

        var documentPath = _pathProvider.GetDocumentPath(projectId, documentId);

        if (!Directory.Exists(documentPath))
            return Task.FromResult(false);

        var documentJsonPath = _pathProvider.GetDocumentJsonPath(projectId, documentId);

        return Task.FromResult(File.Exists(documentJsonPath));
    }

    private static string GetProjectRelativePath(string projectId, string fullPath) {
        var marker = $"{Path.DirectorySeparatorChar}{projectId}{Path.DirectorySeparatorChar}";
        var index = fullPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
            return fullPath;

        return fullPath[(index + marker.Length)..];
    }
}