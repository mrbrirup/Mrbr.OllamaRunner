using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOllamaRunner(builder.Configuration);

using var host = builder.Build();

var manager = host.Services.GetRequiredService<IOllamaInstanceManager>();
var clientFactory = host.Services.GetRequiredService<IOllamaClientFactory>();

await manager.StartAsync("default");

Console.WriteLine("Ollama instance is ready.");

var client = clientFactory.CreateClient("default");
var model = builder.Configuration[
    "OllamaRunner:Instances:default:DefaultModel"];

if (string.IsNullOrWhiteSpace(model))
    throw new InvalidOperationException("No default model configured.");

var reply = await client.ChatAsync(
    model,
    "Say hello in one short sentence.");

Console.WriteLine();
Console.WriteLine("Model response:");
Console.WriteLine(reply);


Console.WriteLine();
Console.WriteLine("Press Enter to stop.");
Console.ReadLine();

await manager.StopAllAsync();