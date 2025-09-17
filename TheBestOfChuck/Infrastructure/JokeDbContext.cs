namespace TheBestOfChuck.Infrastructure;

using Microsoft.EntityFrameworkCore;
using TheBestOfChuck.Domain.Models;

public class JokeDbContext(DbContextOptions<JokeDbContext> options) : DbContext(options)
{
    public DbSet<Joke> Jokes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Joke>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ValueHash).IsUnique();
            entity.Property(e => e.Value).IsRequired();
        });
    }
}
