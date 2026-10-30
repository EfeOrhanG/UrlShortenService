namespace ShortenURL.Models;

public sealed class ShortUrl
{
    public int Id { get; set; }

    public required string OriginalUrl { get; set; }

    public required string ShortCode { get; set; }

    public int AccessCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
