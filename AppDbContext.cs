using Microsoft.EntityFrameworkCore;
using ShortenURL.Models;

namespace ShortenURL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortUrl>(entity =>
        {
            entity.ToTable("short_urls");
            entity.HasKey(shortUrl => shortUrl.Id);

            entity.Property(shortUrl => shortUrl.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(shortUrl => shortUrl.OriginalUrl)
                .HasColumnName("original_url")
                .HasMaxLength(2048)
                .IsRequired();
            entity.Property(shortUrl => shortUrl.ShortCode)
                .HasColumnName("short_code")
                .HasMaxLength(6)
                .IsRequired();
            entity.Property(shortUrl => shortUrl.AccessCount)
                .HasColumnName("access_count")
                .IsRequired();
            entity.Property(shortUrl => shortUrl.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime")
                .IsRequired();
            entity.Property(shortUrl => shortUrl.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("datetime")
                .IsRequired();

            entity.HasIndex(shortUrl => shortUrl.ShortCode)
                .IsUnique();
        });
    }
}
