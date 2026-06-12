using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mrbr.OllamaRunner.Client;
using Mrbr.OllamaRunner.DependencyInjection;
using Mrbr.OllamaRunner.Hosting;
using Mrbr.OllamaRunner.Models.Chat;

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

var chatResponse = await client.ChatAsync(new OllamaChatRequest {
    Model = "llama3.2:1b",
    Messages =
    [
        new OllamaChatMessage
        {
            Role = "user",
            Content = "Say hello in one short sentence."
        }
    ],
    Stream = false
});

Console.WriteLine();
Console.WriteLine("Model response:");
Console.WriteLine(chatResponse.Message?.Content ?? "[No response content]");

Console.WriteLine();
Console.WriteLine("Press Enter to stop.");
Console.ReadLine();

await manager.StopAllAsync();