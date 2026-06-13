using Microsoft.Extensions.Logging.Abstractions;
using Mrbr.OllamaRunner.Storage.Abstractions.Configuration;
using Mrbr.OllamaRunner.Storage.FileSystem.Internal;
using Mrbr.OllamaRunner.Storage.FileSystem.Stores;
using Xunit;

namespace Mrbr.OllamaRunner.Tests.Storage;

public sealed class FileSystemResearchDocumentStoreTests {
    [Fact]
    public async Task ImportTextAsync_CreatesDocumentFiles() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            var document = await store.ImportTextAsync(
                "project-1",
                "Test Document",
                "Hello world.");

            var documentPath = Path.Combine(
                rootPath,
                "Projects",
                "project-1",
                "Documents",
                document.Id);

            Assert.True(Directory.Exists(documentPath));
            Assert.True(File.Exists(Path.Combine(documentPath, "document.json")));
            Assert.True(File.Exists(Path.Combine(documentPath, "original.txt")));
            Assert.True(File.Exists(Path.Combine(documentPath, "extracted.txt")));

            Assert.Equal("project-1", document.ProjectId);
            Assert.Equal("Test Document", document.Name);
            Assert.False(string.IsNullOrWhiteSpace(document.ContentHash));
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task GetAsync_WhenDocumentExists_ReturnsDocument() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            var document = await store.ImportTextAsync(
                "project-1",
                "Test Document",
                "Hello world.");

            var loaded = await store.GetAsync(
                "project-1",
                document.Id);

            Assert.NotNull(loaded);
            Assert.Equal(document.Id, loaded.Id);
            Assert.Equal("Test Document", loaded.Name);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task GetAsync_WhenDocumentDoesNotExist_ReturnsNull() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            var loaded = await store.GetAsync(
                "project-1",
                "missing-document");

            Assert.Null(loaded);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task GetExtractedTextAsync_WhenDocumentExists_ReturnsText() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            var document = await store.ImportTextAsync(
                "project-1",
                "Test Document",
                "Hello world.");

            var text = await store.GetExtractedTextAsync(
                "project-1",
                document.Id);

            Assert.Equal("Hello world.", text);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task ListAsync_ReturnsDocumentsOrderedByName() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            await store.ImportTextAsync("project-1", "Zulu", "Zulu text.");
            await store.ImportTextAsync("project-1", "Alpha", "Alpha text.");

            var documents = await store.ListAsync("project-1");

            Assert.Equal(2, documents.Count);
            Assert.Equal("Alpha", documents[0].Name);
            Assert.Equal("Zulu", documents[1].Name);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task ExistsAsync_WhenDocumentExists_ReturnsTrue() {
        var rootPath = CreateTempRoot();

        try {
            var store = CreateStore(rootPath);

            var document = await store.ImportTextAsync(
                "project-1",
                "Test Document",
                "Hello world.");

            var exists = await store.ExistsAsync(
                "project-1",
                document.Id);

            Assert.True(exists);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    private static FileSystemResearchDocumentStore CreateStore(string rootPath) {
        var options = new OllamaResearchStorageOptions {
            RootPath = rootPath,
            CreateDirectories = true
        };

        var pathProvider = new ResearchStoragePathProvider(options);

        return new FileSystemResearchDocumentStore(
            pathProvider,
            NullLogger<FileSystemResearchDocumentStore>.Instance);
    }

    private static string CreateTempRoot() {
        return Path.Combine(
            Path.GetTempPath(),
            "Mrbr",
            "OllamaRunnerTests",
            Guid.NewGuid().ToString("N"));
    }

    private static void DeleteTempRoot(string rootPath) {
        if (Directory.Exists(rootPath))
            Directory.Delete(rootPath, recursive: true);
    }
}