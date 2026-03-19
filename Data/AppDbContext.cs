using Microsoft.EntityFrameworkCore;
using PasswordGenerator.Models;

namespace PasswordGenerator.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PasswordEntry> PasswordEntries => Set<PasswordEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordEntry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Value).IsRequired().HasMaxLength(256);
            e.Property(x => x.CharSets).HasMaxLength(128);
            e.Property(x => x.StrengthLabel).HasMaxLength(32);
            e.Property(x => x.Note).HasMaxLength(256);
            e.HasIndex(x => x.CreatedAt);
        });
    }
}
