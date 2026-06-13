using Mrbr.OllamaRunner.Storage.Abstractions.Configuration;

namespace Mrbr.OllamaRunner.Storage.FileSystem.Internal;

/// <summary>
/// Provides standard file-system paths for research storage.
/// </summary>
internal sealed class ResearchStoragePathProvider {
    private readonly OllamaResearchStorageOptions _options;

    public ResearchStoragePathProvider(OllamaResearchStorageOptions options) {
        _options = options;
    }

    public string RootPath => _options.RootPath;

    public string ProjectsPath => Path.Combine(RootPath, "Projects");

    public string GetProjectPath(string projectId) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        return Path.Combine(ProjectsPath, projectId);
    }

    public string GetProjectJsonPath(string projectId) {
        return Path.Combine(GetProjectPath(projectId), "project.json");
    }

    public void EnsureRootCreated() {
        if (!_options.CreateDirectories)
            return;

        Directory.CreateDirectory(RootPath);
        Directory.CreateDirectory(ProjectsPath);
    }

    public void EnsureProjectCreated(string projectId) {
        if (!_options.CreateDirectories)
            return;

        Directory.CreateDirectory(GetProjectPath(projectId));
    }
    public string GetDocumentsPath(string projectId) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);

        return Path.Combine(GetProjectPath(projectId), "Documents");
    }

    public string GetDocumentPath(string projectId, string documentId) {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
        ArgumentException.ThrowIfNullOrWhiteSpace(documentId);

        return Path.Combine(GetDocumentsPath(projectId), documentId);
    }

    public string GetDocumentJsonPath(string projectId, string documentId) {
        return Path.Combine(GetDocumentPath(projectId, documentId), "document.json");
    }

    public string GetOriginalTextPath(string projectId, string documentId) {
        return Path.Combine(GetDocumentPath(projectId, documentId), "original.txt");
    }

    public string GetExtractedTextPath(string projectId, string documentId) {
        return Path.Combine(GetDocumentPath(projectId, documentId), "extracted.txt");
    }

    public void EnsureDocumentsCreated(string projectId) {
        if (!_options.CreateDirectories)
            return;

        Directory.CreateDirectory(GetDocumentsPath(projectId));
    }

    public void EnsureDocumentCreated(string projectId, string documentId) {
        if (!_options.CreateDirectories)
            return;

        Directory.CreateDirectory(GetDocumentPath(projectId, documentId));
    }
}