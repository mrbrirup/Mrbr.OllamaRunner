using Microsoft.Extensions.DependencyInjection;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;
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
}