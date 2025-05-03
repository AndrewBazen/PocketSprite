using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using PaletteService.Models;

namespace PaletteService.Data;

internal class PaletteDBContext : DbContext
{
      // Cache the JsonSerializerOptions instance to avoid creating a new one for every operation  
    private static readonly JsonSerializerOptions _cachedJsonSerializerOptions = new JsonSerializerOptions();

    private ValueConverter<List<string>, string> _stringListConverter = new(
        v => JsonSerializer.Serialize(v, _cachedJsonSerializerOptions),
        v => JsonSerializer.Deserialize<List<string>>(v, _cachedJsonSerializerOptions) ?? new List<string>()
    );

    public DbSet<Palette> Palettes { get; set; } = null!;

    public PaletteDBContext(DbContextOptions<PaletteDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
       
        modelBuilder.Entity<Palette>()
            .Property(p => p.Colors)
            .HasConversion(_stringListConverter);

        modelBuilder.Entity<Palette>()
            .Property(p => p.Tags)
            .HasConversion(_stringListConverter);
    }
}
