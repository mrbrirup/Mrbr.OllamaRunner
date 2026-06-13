using Mrbr.OllamaRunner.Hosting;
using Mrbr.OllamaRunner.Models.Common;
using Xunit;

namespace Mrbr.OllamaRunner.Tests;

public sealed class OllamaInstanceManagerTests {
    [Fact]
    public void GetInstance_WhenInstanceExists_ReturnsInstance() {
        var instance = new FakeOllamaInstance("default");
        var manager = new OllamaInstanceManager([instance]);

        var result = manager.GetInstance("default");

        Assert.Same(instance, result);
    }

    [Fact]
    public void GetInstance_IsCaseInsensitive() {
        var instance = new FakeOllamaInstance("default");
        var manager = new OllamaInstanceManager([instance]);

        var result = manager.GetInstance("DEFAULT");

        Assert.Same(instance, result);
    }

    [Fact]
    public void GetInstance_WhenInstanceDoesNotExist_Throws() {
        var manager = new OllamaInstanceManager([
            new FakeOllamaInstance("default")
        ]);

        Assert.Throws<KeyNotFoundException>(() =>
            manager.GetInstance("missing"));
    }

    [Fact]
    public async Task StartAllAsync_StartsEveryInstance() {
        var first = new FakeOllamaInstance("first");
        var second = new FakeOllamaInstance("second");

        var manager = new OllamaInstanceManager([first, second]);

        await manager.StartAllAsync();

        Assert.Equal(1, first.StartCount);
        Assert.Equal(1, second.StartCount);
    }

    [Fact]
    public async Task StopAllAsync_StopsEveryInstance() {
        var first = new FakeOllamaInstance("first");
        var second = new FakeOllamaInstance("second");

        var manager = new OllamaInstanceManager([first, second]);

        await manager.StopAllAsync();

        Assert.Equal(1, first.StopCount);
        Assert.Equal(1, second.StopCount);
    }

    private sealed class FakeOllamaInstance : IOllamaInstance {
        public FakeOllamaInstance(string name) {
            Name = name;
        }

        public string Name { get; }

        public Uri ApiBaseUri { get; } = new("http://127.0.0.1:11434/api/");

        public string? DefaultModel { get; set; } = "llama3.2:1b";

        public string? DefaultKeepAlive { get; set; } = "5m";

        public OllamaRuntimeOptions? DefaultRuntimeOptions { get; set; } = new() {
            Temperature = 0.2,
            NumCtx = 2048
        };

        public bool IsRunning { get; private set; }

        public int StartCount { get; private set; }

        public int StopCount { get; private set; }

        public Task StartAsync(CancellationToken cancellationToken = default) {
            StartCount++;
            IsRunning = true;
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = default) {
            StopCount++;
            IsRunning = false;
            return Task.CompletedTask;
        }

        public Task<bool> WaitUntilReadyAsync(CancellationToken cancellationToken = default) {
            return Task.FromResult(true);
        }

        public ValueTask DisposeAsync() {
            return ValueTask.CompletedTask;
        }
        public string? DefaultEmbeddingModel { get; set; } = "nomic-embed-text";

        public int? DefaultEmbeddingDimensions { get; set; }

        public bool? DefaultEmbeddingTruncate { get; set; } = true;
    }
}