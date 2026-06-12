using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Configuration;
using System.Diagnostics;

namespace Mrbr.OllamaRunner.Hosting;

/// <summary>
/// Starts Ollama server processes.
/// </summary>
public sealed class OllamaProcessRunner {
    private readonly OllamaRunnerOptions _runnerOptions;
    private readonly ILogger<OllamaProcessRunner> _logger;

    public OllamaProcessRunner(
        OllamaRunnerOptions runnerOptions,
        ILogger<OllamaProcessRunner> logger) {
        _runnerOptions = runnerOptions;
        _logger = logger;
    }

    /// <summary>
    /// Starts an "ollama serve" process for the supplied instance.
    /// </summary>
    public Process Start(OllamaInstanceOptions instanceOptions) {
        ArgumentNullException.ThrowIfNull(instanceOptions);

        string fileName = string.IsNullOrWhiteSpace(_runnerOptions.OllamaExecutablePath)
            ? "ollama"
            : _runnerOptions.OllamaExecutablePath;

        var startInfo = new ProcessStartInfo {
            FileName = fileName,
            Arguments = "serve",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.Environment["OLLAMA_HOST"] = $"{instanceOptions.BindHost}:{instanceOptions.Port}";

        if (!string.IsNullOrWhiteSpace(instanceOptions.ModelsPath))
            startInfo.Environment["OLLAMA_MODELS"] = instanceOptions.ModelsPath;

        foreach ((string key, string value) in instanceOptions.EnvironmentVariables)
            startInfo.Environment[key] = value;

        _logger.LogInformation(
            "Starting Ollama instance {InstanceName} on {Host}:{Port}",
            instanceOptions.Name,
            instanceOptions.BindHost,
            instanceOptions.Port);

        var process = new Process {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (_, args) => {
            if (!string.IsNullOrWhiteSpace(args.Data))
                _logger.LogInformation("[ollama:{InstanceName}] {Message}", instanceOptions.Name, args.Data);
        };

        process.ErrorDataReceived += (_, args) => {
            if (!string.IsNullOrWhiteSpace(args.Data))
                _logger.LogWarning("[ollama:{InstanceName}] {Message}", instanceOptions.Name, args.Data);
        };

        if (!process.Start())
            throw new InvalidOperationException($"Failed to start Ollama instance '{instanceOptions.Name}'.");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        return process;
    }
}