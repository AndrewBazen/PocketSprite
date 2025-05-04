/* PaletteDbContext.cs
*
* @description: DbContext for the PaletteService
*
* @author: Andrew Bazen <github.com/AndrewBazen>
* @version: 1.0
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*/
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using PaletteService.Models;
namespace PaletteService.Data;

/* @class: PaletteDbContext
*
* @description: DbContext for the PaletteService
* @date-created: 2025-05-03
* @date-modified: 2025-05-03
*/
public class PaletteDbContext : DbContext
{
    // Cache the JsonSerializerOptions instance to avoid creating a new one for every operation  
    private static readonly JsonSerializerOptions _cachedJsonSerializerOptions = new JsonSerializerOptions();

    private ValueConverter<List<string>, string> _nonNullableListConverter = new(
        v => JsonSerializer.Serialize(v, _cachedJsonSerializerOptions),
        v => JsonSerializer.Deserialize<List<string>>(v, _cachedJsonSerializerOptions) ?? new List<string>()
    );

    private ValueConverter<List<string>?, string> _nullableListConverter = new(
        v => JsonSerializer.Serialize(v ?? new List<string>(), _cachedJsonSerializerOptions),
        v => JsonSerializer.Deserialize<List<string>>(v, _cachedJsonSerializerOptions)
    );

    private ValueComparer<List<string>> listComparer = new ValueComparer<List<string>>(
            (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
            c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c == null ? new List<string>() : c.ToList()
    );

    public DbSet<Palette> Palettes { get; set; } = null!;

    /* @constructor: PaletteDbContext
    *
    * @description: Constructor for the PaletteDbContext
    * @param: DbContextOptions<PaletteDbContext> options
    * @return: void
    */
    public PaletteDbContext(DbContextOptions<PaletteDbContext> options) : base(options) { }

    /* @method: OnModelCreating
    *
    * @description: Configures the model for the PaletteDbContext
    * @param: ModelBuilder modelBuilder
    * @return: void
    */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Palette>()
            .Property(p => p.Colors)
            .HasConversion(_nonNullableListConverter)
            .Metadata.SetValueComparer(listComparer);

        modelBuilder.Entity<Palette>()
            .Property(p => p.Tags)
            .HasConversion(_nullableListConverter)
            .Metadata.SetValueComparer(listComparer);
    }
}
