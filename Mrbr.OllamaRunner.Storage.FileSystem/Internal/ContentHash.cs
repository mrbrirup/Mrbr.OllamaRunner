using System.Security.Cryptography;
using System.Text;

namespace Mrbr.OllamaRunner.Storage.FileSystem.Internal;

internal static class ContentHash {
    public static string Sha256Hex(string content) {
        ArgumentNullException.ThrowIfNull(content);

        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}