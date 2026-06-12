using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Configuration;
using System.Diagnostics;

namespace Mrbr.OllamaRunner.Hosting;

/// <summary>
/// Runtime representation of a configured Ollama instance.
/// </summary>
public sealed class OllamaInstance : IOllamaInstance {
    private readonly OllamaInstanceOptions _options;
    private readonly OllamaProcessRunner _processRunner;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaInstance> _logger;

    private Process? _process;

    public OllamaInstance(
        OllamaInstanceOptions options,
        OllamaProcessRunner processRunner,
        HttpClient httpClient,
        ILogger<OllamaInstance> logger) {
        _options = options;
        _processRunner = processRunner;
        _httpClient = httpClient;
        _logger = logger;

        Name = options.Name;
        ApiBaseUri = options.ApiBaseUri;
    }

    public string Name { get; }

    public Uri ApiBaseUri { get; }

    public bool IsRunning => _process is { HasExited: false };

    public string? DefaultModel => _options.DefaultModel;

    public async Task StartAsync(CancellationToken cancellationToken = default) {
        if (_options.Mode == OllamaInstanceMode.ManagedProcess && !IsRunning)
            _process = _processRunner.Start(_options);

        bool ready = await WaitUntilReadyAsync(cancellationToken);

        if (!ready)
            throw new TimeoutException($"Ollama instance '{Name}' did not become ready at {ApiBaseUri}.");

        _logger.LogInformation("Ollama instance {InstanceName} is ready at {ApiBaseUri}", Name, ApiBaseUri);
    }

    public async Task<bool> WaitUntilReadyAsync(CancellationToken cancellationToken = default) {
        var timeout = TimeSpan.FromSeconds(_options.StartupTimeoutSeconds);
        var deadline = DateTimeOffset.UtcNow.Add(timeout);

        while (DateTimeOffset.UtcNow < deadline) {
            cancellationToken.ThrowIfCancellationRequested();

            try {
                using var response = await _httpClient.GetAsync("tags", cancellationToken);

                if (response.IsSuccessStatusCode)
                    return true;
            }
            catch (HttpRequestException) {
                // Expected while Ollama is starting.
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested) {
                // Ignore transient HTTP timeout.
            }

            await Task.Delay(500, cancellationToken);
        }

        return false;
    }

    public async Task StopAsync(CancellationToken cancellationToken = default) {
        if (_options.Mode == OllamaInstanceMode.External)
            return;

        if (_process is null)
            return;

        if (_process.HasExited) {
            _process.Dispose();
            _process = null;
            return;
        }

        _logger.LogInformation("Stopping Ollama instance {InstanceName}", Name);

        try {
            _process.Kill(entireProcessTree: true);

            await _process.WaitForExitAsync(cancellationToken);
        }
        finally {
            _process.Dispose();
            _process = null;
        }
    }

    public async ValueTask DisposeAsync() {
        await StopAsync();
        _httpClient.Dispose();
    }
}