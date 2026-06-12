using Microsoft.Extensions.DependencyInjection;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;
using Xunit;

namespace Mrbr.OllamaRunner.Tests.Integration;

public sealed class OllamaIntegrationTests {
    [Fact]
    public async Task ChatAsync_WithRealOllama_WhenEnabled_ReturnsResponse() {
        if (!IsEnabled())
            return;

        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "10"
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var clientFactory = provider.GetRequiredService<IOllamaClientFactory>();

        var instance = manager.GetInstance("default");

        await instance.StartAsync();

        var model = instance.DefaultModel
            ?? throw new InvalidOperationException("No default model configured.");

        var client = clientFactory.CreateClient("default");

        var reply = await client.ChatAsync(
            model,
            "Reply with exactly one word: Hello");

        Assert.False(string.IsNullOrWhiteSpace(reply));
    }

    private static bool IsEnabled() {
        return string.Equals(
            Environment.GetEnvironmentVariable("MRBR_OLLAMA_INTEGRATION_TESTS"),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }
}