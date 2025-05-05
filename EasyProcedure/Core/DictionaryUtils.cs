using System.Security.Cryptography;
using System.Text;

namespace EasyProcedure.Core;

public static class DictionaryUtils
{
    public static string GenerateStringKey(params string[] keyParameters)
    {
        if (keyParameters.Length == 0)
            return string.Empty;

        var combined = string.Join("||", keyParameters);
        var inputBytes = Encoding.UTF8.GetBytes(combined);
        var hashBytes = SHA256.HashData(inputBytes);

        var base64 = Convert.ToBase64String(hashBytes);

        // Make it URL-safe
        base64 = base64.Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return base64; // Max 44 characters for SHA-256
        // (no problem with telegram callback data length limit: 64 > 44)
    }
}