using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.Configuration;
using Mrbr.OllamaRunner.Hosting;
namespace Mrbr.OllamaRunner.DependencyInjection;

/// <summary>
/// Dependency injection registration for the Ollama runner.
/// </summary>
public static class OllamaRunnerServiceCollectionExtensions {
    public static IServiceCollection AddOllamaRunner(
        this IServiceCollection services,
        IConfiguration configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration
            .GetSection("OllamaRunner")
            .Get<OllamaRunnerOptions>()

            ?? throw new InvalidOperationException("Missing OllamaRunner configuration section.");

        NormaliseInstanceNames(options);
        ValidateOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<OllamaProcessRunner>();

        foreach (var instanceOptions in options.Instances.Values) {
            services.AddSingleton<IOllamaInstance>(provider => {
                var processRunner = provider.GetRequiredService<OllamaProcessRunner>();
                var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<OllamaInstance>>();

                var httpClient = new HttpClient {
                    BaseAddress = instanceOptions.ApiBaseUri,
                    Timeout = TimeSpan.FromSeconds(10)
                };

                return new OllamaInstance(
                    instanceOptions,
                    processRunner,
                    httpClient,
                    logger);
            });
        }

        services.AddSingleton<IOllamaInstanceManager, OllamaInstanceManager>();
        services.AddSingleton<IOllamaClientFactory, OllamaClientFactory>();
        return services;
    }

    private static void NormaliseInstanceNames(OllamaRunnerOptions options) {
        foreach ((string key, OllamaInstanceOptions instance) in options.Instances) {
            if (string.IsNullOrWhiteSpace(instance.Name))
                instance.Name = key;
        }
    }

    private static void ValidateOptions(OllamaRunnerOptions options) {
        if (options.Instances.Count == 0)
            throw new InvalidOperationException("At least one Ollama instance must be configured.");

        foreach (var instance in options.Instances.Values) {
            if (string.IsNullOrWhiteSpace(instance.Name))
                throw new InvalidOperationException("Every Ollama instance must have a name.");

            if (string.IsNullOrWhiteSpace(instance.BindHost))
                throw new InvalidOperationException($"Ollama instance '{instance.Name}' has no bind host.");

            if (instance.Port <= 0 || instance.Port > 65535)
                throw new InvalidOperationException($"Ollama instance '{instance.Name}' has invalid port '{instance.Port}'.");

            if (instance.StartupTimeoutSeconds <= 0)
                throw new InvalidOperationException($"Ollama instance '{instance.Name}' must have a positive startup timeout.");
        }
    }
}