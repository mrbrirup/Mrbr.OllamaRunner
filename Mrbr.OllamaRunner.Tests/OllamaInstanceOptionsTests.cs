using Mrbr.OllamaRunner.Configuration;
using Xunit;

namespace Mrbr.OllamaRunner.Tests;

public sealed class OllamaInstanceOptionsTests {
    [Fact]
    public void ApiBaseUri_ReturnsExpectedUri() {
        var options = new OllamaInstanceOptions {
            Name = "default",
            BindHost = "127.0.0.1",
            Port = 11434
        };

        Assert.Equal(
            new Uri("http://127.0.0.1:11434/api/"),
            options.ApiBaseUri);
    }

    [Fact]
    public void ApiBaseUri_UsesConfiguredPort() {
        var options = new OllamaInstanceOptions {
            Name = "research",
            BindHost = "127.0.0.1",
            Port = 11436
        };

        Assert.Equal(
            new Uri("http://127.0.0.1:11436/api/"),
            options.ApiBaseUri);
    }

    [Fact]
    public void Defaults_AreExpected() {
        var options = new OllamaInstanceOptions();

        Assert.Equal(OllamaInstanceMode.ManagedProcess, options.Mode);
        Assert.Equal("127.0.0.1", options.BindHost);
        Assert.Equal(11434, options.Port);
        Assert.Equal(30, options.StartupTimeoutSeconds);
    }
}