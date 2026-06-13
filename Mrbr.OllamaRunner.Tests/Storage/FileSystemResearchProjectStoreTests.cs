using Microsoft.Extensions.Logging.Abstractions;
using Mrbr.OllamaRunner.Storage.Abstractions.Configuration;
using Mrbr.OllamaRunner.Storage.FileSystem.Internal;
using Mrbr.OllamaRunner.Storage.FileSystem.Stores;
using Xunit;

namespace Mrbr.OllamaRunner.Tests.Storage;

public sealed class FileSystemResearchProjectStoreTests {
    [Fact]
    public async Task CreateAsync_CreatesProjectJson() {
        var rootPath = CreateTempRoot();
        try {

            var store = CreateStore(rootPath);

            var project = await store.CreateAsync(
                "Test Project",
                "Test Description");

            var projectJsonPath = Path.Combine(
                rootPath,
                "Projects",
                project.Id,
                "project.json");

            Assert.True(File.Exists(projectJsonPath));
            Assert.Equal("Test Project", project.Name);
            Assert.Equal("Test Description", project.Description);

        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task GetAsync_WhenProjectExists_ReturnsProject() {
        var rootPath = CreateTempRoot();
        try {
            var store = CreateStore(rootPath);

            var project = await store.CreateAsync("Test Project");

            var loaded = await store.GetAsync(project.Id);

            Assert.NotNull(loaded);
            Assert.Equal(project.Id, loaded.Id);
            Assert.Equal("Test Project", loaded.Name);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task GetAsync_WhenProjectDoesNotExist_ReturnsNull() {
        var rootPath = CreateTempRoot();
        try {

            var store = CreateStore(rootPath);

            var loaded = await store.GetAsync("missing-project");

            Assert.Null(loaded);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task ListAsync_ReturnsSavedProjectsOrderedByName() {
        var rootPath = CreateTempRoot();
        try {

            var store = CreateStore(rootPath);

            await store.CreateAsync("Zulu Project");
            await store.CreateAsync("Alpha Project");

            var projects = await store.ListAsync();

            Assert.Equal(2, projects.Count);
            Assert.Equal("Alpha Project", projects[0].Name);
            Assert.Equal("Zulu Project", projects[1].Name);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    [Fact]
    public async Task ExistsAsync_WhenProjectExists_ReturnsTrue() {
        var rootPath = CreateTempRoot();
        try {
            var store = CreateStore(rootPath);

            var project = await store.CreateAsync("Test Project");

            var exists = await store.ExistsAsync(project.Id);

            Assert.True(exists);
        }
        finally {
            DeleteTempRoot(rootPath);
        }
    }

    private static FileSystemResearchProjectStore CreateStore(string rootPath) {
        var options = new OllamaResearchStorageOptions {
            RootPath = rootPath,
            CreateDirectories = true
        };

        var pathProvider = new ResearchStoragePathProvider(options);

        return new FileSystemResearchProjectStore(
            pathProvider,
            NullLogger<FileSystemResearchProjectStore>.Instance);
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