using System.Security.Cryptography;

namespace CarDealer.API.Shared.Common;

public static class TokenGenerator
{
    // 32 بايت = 256 بت عشوائية حقيقية، غير قابلة للتخمين إطلاقًا
    public static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");   // Base64Url-safe، صالح مباشرة داخل رابط
    }
}