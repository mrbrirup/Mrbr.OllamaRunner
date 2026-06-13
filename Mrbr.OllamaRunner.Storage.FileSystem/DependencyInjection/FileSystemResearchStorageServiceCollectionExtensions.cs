using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Storage.Abstractions.Configuration;
using Mrbr.OllamaRunner.Storage.Abstractions.Stores;
using Mrbr.OllamaRunner.Storage.FileSystem.Internal;
using Mrbr.OllamaRunner.Storage.FileSystem.Stores;

namespace Mrbr.OllamaRunner.Storage.FileSystem.DependencyInjection;

/// <summary>
/// Dependency injection registration for file-system research storage.
/// </summary>
public static class FileSystemResearchStorageServiceCollectionExtensions {
    public static IServiceCollection AddFileSystemResearchStorage(
        this IServiceCollection services,
        IConfiguration configuration) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration
            .GetSection("OllamaRunner:ResearchStorage")
            .Get<OllamaResearchStorageOptions>()
            ?? throw new InvalidOperationException(
                "Missing OllamaRunner:ResearchStorage configuration section.");

        ValidateOptions(options);

        services.AddSingleton(options);
        services.AddSingleton<ResearchStoragePathProvider>();
        services.AddSingleton<IResearchProjectStore>(provider => {
            var pathProvider = provider.GetRequiredService<ResearchStoragePathProvider>();
            var logger = provider.GetRequiredService<ILogger<FileSystemResearchProjectStore>>();

            return new FileSystemResearchProjectStore(
                pathProvider,
                logger);
        });

        return services;
    }

    private static void ValidateOptions(OllamaResearchStorageOptions options) {
        if (string.IsNullOrWhiteSpace(options.RootPath))
            throw new InvalidOperationException("Research storage RootPath is required.");
    }
}