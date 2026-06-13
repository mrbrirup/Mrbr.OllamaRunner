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
    var models = await client.ListModelsAsync(cancellationTokenSource.Token);
    var hasDefaultModel = models.Models.Any(
installedModel => string.Equals(
    installedModel.Name,
    model,
    StringComparison.OrdinalIgnoreCase)
|| string.Equals(
    installedModel.Model,
    model,
    StringComparison.OrdinalIgnoreCase));

    if (!hasDefaultModel) {
        Console.WriteLine();
        Console.WriteLine($"Warning: default model '{model}' was not found in the local model list.");
    }
    if (!models.ContainsModel(model)) {
        Console.WriteLine();
        Console.WriteLine($"Warning: default model '{model}' was not found in the local model list.");
    }
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
    Console.WriteLine("Streaming response:");

    await foreach (var chunk in client.ChatStreamAsync(
        model,
        "Write one short paragraph about local AI.",
        instance.DefaultRuntimeOptions,
        instance.DefaultKeepAlive,
        cancellationTokenSource.Token)) {
        Console.Write(chunk.Message?.Content);
    }

    Console.WriteLine();

    Console.WriteLine();
    Console.WriteLine("Generate response:");

    var generated = await client.GenerateAsync(
        model,
        "Write one short sentence describing a local Ollama runner.",
        instance.DefaultRuntimeOptions,
        instance.DefaultKeepAlive,
        cancellationTokenSource.Token);

    Console.WriteLine(generated);

    Console.WriteLine();
    Console.WriteLine("Generate streaming response:");

    await foreach (var chunk in client.GenerateStreamAsync(
        model,
        "Write one short paragraph describing why local AI is useful.",
        instance.DefaultRuntimeOptions,
        instance.DefaultKeepAlive,
        cancellationTokenSource.Token)) {
        Console.Write(chunk.Response);
    }

    Console.WriteLine();



    Console.WriteLine();
    Console.WriteLine("Installed models:");

    foreach (var installedModel in models.Models) {
        Console.WriteLine($"- {installedModel.Name}");
    }


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