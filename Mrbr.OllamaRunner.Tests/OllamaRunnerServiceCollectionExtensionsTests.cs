using Microsoft.Extensions.DependencyInjection;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;
using Mrbr.OllamaRunner.Models.Common;
using System.Net;
using System.Text;
using Xunit;

namespace Mrbr.OllamaRunner.Tests;

public sealed class OllamaRunnerServiceCollectionExtensionsTests {
    [Fact]
    public async Task AddOllamaRunner_RegistersInstanceManager() {
        var configuration = TestConfigurationFactory.CreateValidSingleInstanceConfiguration();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();

        Assert.NotNull(manager);
        Assert.Single(manager.Instances);
    }

    [Fact]
    public async Task AddOllamaRunner_NormalisesInstanceNameFromDictionaryKey() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:research:Mode"] = "External",
            ["OllamaRunner:Instances:research:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:research:Port"] = "11436",
            ["OllamaRunner:Instances:research:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:research:StartupTimeoutSeconds"] = "30"
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("research");

        Assert.Equal("research", instance.Name);
        Assert.Equal("llama3.2:1b", instance.DefaultModel);
    }

    [Fact]
    public async Task AddOllamaRunner_ExposesDefaultModelThroughInstance() {
        var configuration = TestConfigurationFactory.CreateValidSingleInstanceConfiguration();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("default");

        Assert.Equal("llama3.2:1b", instance.DefaultModel);
    }

    [Fact]
    public async Task AddOllamaRunner_UsesConfiguredApiBaseUri() {
        var configuration = TestConfigurationFactory.CreateValidSingleInstanceConfiguration();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("default");

        Assert.Equal(
            new Uri("http://127.0.0.1:11434/api/"),
            instance.ApiBaseUri);
    }

    [Fact]
    public void AddOllamaRunner_WithNoInstances_Throws() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:OllamaExecutablePath"] = "ollama"
        });

        var services = new ServiceCollection();

        services.AddLogging();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddOllamaRunner(configuration));

        Assert.Contains("At least one Ollama instance", exception.Message);
    }

    [Fact]
    public void AddOllamaRunner_WithInvalidPort_Throws() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "99999",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "30"
        });

        var services = new ServiceCollection();

        services.AddLogging();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddOllamaRunner(configuration));

        Assert.Contains("invalid port", exception.Message);
    }

    [Fact]
    public void AddOllamaRunner_WithInvalidStartupTimeout_Throws() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "0"
        });

        var services = new ServiceCollection();

        services.AddLogging();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.AddOllamaRunner(configuration));

        Assert.Contains("positive startup timeout", exception.Message);
    }
    [Fact]
    public async Task AddOllamaRunner_ExposesDefaultKeepAliveThroughInstance() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:default:DefaultKeepAlive"] = "5m",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "30"
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("default");

        Assert.Equal("5m", instance.DefaultKeepAlive);
    }
    [Fact]
    public async Task AddOllamaRunner_ExposesDefaultRuntimeOptionsThroughInstance() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:default:DefaultKeepAlive"] = "5m",
            ["OllamaRunner:Instances:default:DefaultRuntimeOptions:Temperature"] = "0.2",
            ["OllamaRunner:Instances:default:DefaultRuntimeOptions:NumCtx"] = "2048",
            ["OllamaRunner:Instances:default:DefaultRuntimeOptions:TopP"] = "0.9",
            ["OllamaRunner:Instances:default:DefaultRuntimeOptions:TopK"] = "40",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "30"
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("default");

        Assert.NotNull(instance.DefaultRuntimeOptions);
        Assert.Equal(0.2, instance.DefaultRuntimeOptions.Temperature);
        Assert.Equal(2048, instance.DefaultRuntimeOptions.NumCtx);
        Assert.Equal(0.9, instance.DefaultRuntimeOptions.TopP);
        Assert.Equal(40, instance.DefaultRuntimeOptions.TopK);
    }
    [Fact]
    public async Task ChatAsync_WithOptions_SendsOptionsAndKeepAlive() {
        string? requestJson = null;

        var handler = new FakeHttpMessageHandler((request, _) => {
            requestJson = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(
                    """
                {
                  "model": "llama3.2:1b",
                  "message": {
                    "role": "assistant",
                    "content": "Hello"
                  },
                  "done": true
                }
                """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        var httpClient = new HttpClient(handler) {
            BaseAddress = new Uri("http://127.0.0.1:11434/api/")
        };

        var client = new OllamaClient(httpClient);

        var reply = await client.ChatAsync(
            "llama3.2:1b",
            "Say hello.",
            new OllamaRuntimeOptions {
                Temperature = 0.2,
                NumCtx = 2048
            },
            keepAlive: "5m");

        Assert.Equal("Hello", reply);
        Assert.NotNull(requestJson);

        Assert.Contains("\"keep_alive\":\"5m\"", requestJson);
        Assert.Contains("\"temperature\":0.2", requestJson);
        Assert.Contains("\"num_ctx\":2048", requestJson);
    }
    [Fact]
    public async Task AddOllamaRunner_ExposesDefaultEmbeddingSettingsThroughInstance() {
        var configuration = TestConfigurationFactory.Create(new Dictionary<string, string?> {
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:default:DefaultEmbeddingModel"] = "nomic-embed-text",
            ["OllamaRunner:Instances:default:DefaultEmbeddingTruncate"] = "true",
            ["OllamaRunner:Instances:default:DefaultEmbeddingDimensions"] = "768",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "30"
        });

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddOllamaRunner(configuration);

        await using var provider = services.BuildServiceProvider();

        var manager = provider.GetRequiredService<IOllamaInstanceManager>();
        var instance = manager.GetInstance("default");

        Assert.Equal("nomic-embed-text", instance.DefaultEmbeddingModel);
        Assert.True(instance.DefaultEmbeddingTruncate);
        Assert.Equal(768, instance.DefaultEmbeddingDimensions);
    }
}