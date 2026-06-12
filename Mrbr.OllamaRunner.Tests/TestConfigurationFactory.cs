using Microsoft.Extensions.Configuration;

namespace Mrbr.OllamaRunner.Tests;

internal static class TestConfigurationFactory {
    public static IConfiguration Create(Dictionary<string, string?> values) {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    public static IConfiguration CreateValidSingleInstanceConfiguration() {
        return Create(new Dictionary<string, string?> {
            ["OllamaRunner:OllamaExecutablePath"] = "ollama",
            ["OllamaRunner:Instances:default:Name"] = "default",
            ["OllamaRunner:Instances:default:Mode"] = "External",
            ["OllamaRunner:Instances:default:BindHost"] = "127.0.0.1",
            ["OllamaRunner:Instances:default:Port"] = "11434",
            ["OllamaRunner:Instances:default:ModelsPath"] = @"C:\Users\mrbr\.ollama\models",
            ["OllamaRunner:Instances:default:DefaultModel"] = "llama3.2:1b",
            ["OllamaRunner:Instances:default:StartupTimeoutSeconds"] = "30"
        });
    }
}