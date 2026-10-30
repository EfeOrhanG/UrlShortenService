using System.ComponentModel.DataAnnotations;

namespace ShortenURL.Contracts;

/// <summary>Request body for creating a shortened URL.</summary>
public sealed record CreateShortUrlRequest
{
    /// <summary>The absolute HTTP or HTTPS URL to shorten.</summary>
    [Required]
    public required string Url { get; init; }
}

/// <summary>Request body for replacing a shortened URL's destination.</summary>
public sealed record UpdateShortUrlRequest
{
    /// <summary>The new absolute HTTP or HTTPS destination URL.</summary>
    [Required]
    public required string Url { get; init; }
}

/// <summary>Shortened URL information returned by the API.</summary>
public sealed record ShortUrlResponse(
    int Id,
    string Url,
    string ShortCode,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>Access count for a shortened URL.</summary>
public sealed record ShortUrlStatsResponse(
    int Id,
    string Url,
    string ShortCode,
    int AccessCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
    );
