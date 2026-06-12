# Mrbr.OllamaRunner

Reusable .NET library for configuring, starting, stopping and calling local Ollama instances.

## Milestone 1

Implemented:

- Named Ollama instance configuration
- Managed-process mode
- External mode
- Per-instance `OLLAMA_HOST`
- Optional per-instance `OLLAMA_MODELS`
- Readiness check using `/api/tags`
- Non-streaming `/api/chat`
- Default model configuration
- Unit tests for configuration, instance manager and client

## Example configuration

```json
{
  "OllamaRunner": {
    "OllamaExecutablePath": "C:\\Users\\mrbr\\AppData\\Local\\Programs\\Ollama\\ollama.exe",
    "Instances": {
      "default": {
        "Name": "default",
        "Mode": "ManagedProcess",
        "BindHost": "127.0.0.1",
        "Port": 11434,
        "ModelsPath": "C:\\Users\\mrbr\\.ollama\\models",
        "DefaultModel": "llama3.2:1b",
        "StartupTimeoutSeconds": 30
      }
    }
  }
}