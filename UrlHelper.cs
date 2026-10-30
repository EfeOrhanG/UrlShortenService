using System;

namespace ShortenURL.UrlHelper;

public class UrlHelper
{
    public static bool IsValidUrl(string url)
    {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (url.Length > 2048)
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            return uri.Scheme == Uri.UriSchemeHttp ||
                uri.Scheme == Uri.UriSchemeHttps;
    }

    public static string GenerateShortCode()
    {   
         const string chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        var result = new char[6];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
        }

        return new string(result);
        
    }
}
