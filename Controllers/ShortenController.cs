using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShortenURL.Contracts;
using ShortenURL.Models;
using ShortenURL.UrlHelper;

namespace ShortenURL.Controllers
{
    [ApiController]
    [Route("api")]
    public class ShortenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShortenController(AppDbContext context)
        {
            _context = context;
        }



        [HttpPost]
        [Route("shorten")]
        [ProducesResponseType<ShortUrlResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ShortUrl(
            [FromBody] CreateShortUrlRequest request,
            CancellationToken cancellationToken)
        {
            if (!UrlHelper.UrlHelper.IsValidUrl(request.Url))
            {
                ModelState.AddModelError(nameof(request.Url), "A valid absolute HTTP or HTTPS URL is required.");
                return ValidationProblem(ModelState);
            }

            for (var attempt = 0; attempt < 20; attempt++)
            {
                var shortCode = UrlHelper.UrlHelper.GenerateShortCode();
                if (await _context.ShortUrls.AnyAsync(
                        item => item.ShortCode == shortCode,
                        cancellationToken))
                {
                    continue;
                }

                var now = DateTime.UtcNow;
                var shortUrl = new ShortUrl
                {
                    OriginalUrl = request.Url,
                    ShortCode = shortCode,
                    AccessCount = 0,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                _context.ShortUrls.Add(shortUrl);
                await _context.SaveChangesAsync(cancellationToken);

                var response = ToResponse(shortUrl);
                return CreatedAtAction(
                    nameof(GetOriginalUrl),
                    new { shortCode = response.ShortCode },
                    response);
            }

            return Conflict(new ProblemDetails
            {
                Title = "Unable to generate a unique short code.",
                Status = StatusCodes.Status409Conflict
            });
        }

        [HttpGet]
        [Route("shorten/{shortCode}/stats")]
        [ProducesResponseType<ShortUrlStatsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShortUrlStatsResponse>> getStats(
            string shortCode,
            CancellationToken cancellationToken)
        {
            var data = await _context.ShortUrls
                .AsNoTracking()
                .Where(item => item.ShortCode == shortCode)
                .SingleOrDefaultAsync(cancellationToken);

            if (data is null)
            {
                return NotFound();
            }

            return Ok(new ShortUrlStatsResponse(data.Id, data.OriginalUrl, data.ShortCode, data.AccessCount, data.CreatedAt, data.UpdatedAt));
        }

        [HttpGet]
        [Route("shorten/{shortCode}")]
        [ProducesResponseType<ShortUrlResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShortUrlResponse>> GetOriginalUrl(
            string shortCode,
            CancellationToken cancellationToken)
        {
            var shortUrl = await _context.ShortUrls
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.ShortCode == shortCode, cancellationToken);

            return shortUrl is null ? NotFound() : Ok(ToResponse(shortUrl));
        }

        [HttpGet("{shortCode}")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RedirectToOriginal(
            string shortCode,
            CancellationToken cancellationToken)
        {
            var updatedCount = await _context.ShortUrls
                .Where(item => item.ShortCode == shortCode)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(
                        item => item.AccessCount,
                        item => item.AccessCount + 1),
                    cancellationToken);

            if (updatedCount == 0)
            {
                return NotFound();
            }

            var originalUrl = await _context.ShortUrls
                .AsNoTracking()
                .Where(item => item.ShortCode == shortCode)
                .Select(item => item.OriginalUrl)
                .SingleOrDefaultAsync(cancellationToken);

            return originalUrl is null ? NotFound() : Redirect(originalUrl);
        }

        [HttpPut]
        [Route("shorten/{shortCode}")]
        [ProducesResponseType<ShortUrlResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ShortUrlResponse>> UpdateUrl(
            string shortCode,
            [FromBody] UpdateShortUrlRequest request,
            CancellationToken cancellationToken)
        {
            if (!UrlHelper.UrlHelper.IsValidUrl(request.Url))
            {
                ModelState.AddModelError(nameof(request.Url), "A valid absolute HTTP or HTTPS URL is required.");
                return ValidationProblem(ModelState);
            }

            var shortUrl = await _context.ShortUrls
                .SingleOrDefaultAsync(item => item.ShortCode == shortCode, cancellationToken);

            if (shortUrl is null)
            {
                return NotFound();
            }

            shortUrl.OriginalUrl = request.Url;
            shortUrl.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ToResponse(shortUrl));
        }

        [HttpDelete]
        [Route("shorten/{shortCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUrl(
            string shortCode,
            CancellationToken cancellationToken)
        {
            var deletedCount = await _context.ShortUrls
                .Where(item => item.ShortCode == shortCode)
                .ExecuteDeleteAsync(cancellationToken);

            return deletedCount > 0 ? NoContent() : NotFound();
        }


        private static ShortUrlResponse ToResponse(ShortUrl shortUrl) =>
            new(
                shortUrl.Id,
                shortUrl.OriginalUrl,
                shortUrl.ShortCode,
                new DateTimeOffset(DateTime.SpecifyKind(shortUrl.CreatedAt, DateTimeKind.Utc)),
                new DateTimeOffset(DateTime.SpecifyKind(shortUrl.UpdatedAt, DateTimeKind.Utc)));
    }
}
