using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;

using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, args) => {
    args.Cancel = true;
    cancellationTokenSource.Cancel();
};

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOllamaRunner(builder.Configuration);

using var host = builder.Build();

var manager = host.Services.GetRequiredService<IOllamaInstanceManager>();
var clientFactory = host.Services.GetRequiredService<IOllamaClientFactory>();

try {
    var instance = manager.GetInstance("default");

    await instance.StartAsync(cancellationTokenSource.Token);

    var model = instance.DefaultModel
        ?? throw new InvalidOperationException(
            $"No default model configured for Ollama instance '{instance.Name}'.");

    var client = clientFactory.CreateClient(instance.Name);

    var reply = await client.ChatAsync(
    model,
    "Say hello in one short sentence.",
    instance.DefaultRuntimeOptions,
    instance.DefaultKeepAlive,
    cancellationTokenSource.Token);

    Console.WriteLine();
    Console.WriteLine("Model response:");
    Console.WriteLine(reply);

    Console.WriteLine();
    Console.WriteLine("Press Enter or Ctrl+C to stop.");

    while (!cancellationTokenSource.IsCancellationRequested) {
        if (Console.KeyAvailable && Console.ReadKey(intercept: true).Key == ConsoleKey.Enter)
            break;

        await Task.Delay(100, cancellationTokenSource.Token)
            .WaitAsync(TimeSpan.FromMilliseconds(150));
    }
}
catch (OperationCanceledException) {
    Console.WriteLine();
    Console.WriteLine("Shutdown requested.");
}
finally {
    await manager.StopAllAsync();
}