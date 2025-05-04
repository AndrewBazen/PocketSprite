using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Npgsql;
using PaletteService.Data;
using PaletteService.Models;
using System.Diagnostics;
using Xunit.Abstractions;

namespace PocketSpriteTest;

public class APITest : IAsyncLifetime, IDisposable
{
    private PostgreSqlContainer _postgresContainer = default!;
    private PaletteDbContext _context = default!;
    private HttpClient _client = default!;
    private string _connectionString = default!;
    private readonly ITestOutputHelper _output;

    public APITest(ITestOutputHelper output)
    {
        _output = output;
    }

    public async Task InitializeAsync()
    {
        // Create and start the container
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("palette_test")
            .WithUsername("postgres")
            .WithPassword("password")
            .WithImage("postgres:16")
            .WithCleanUp(true)
            .Build();

        await _postgresContainer.StartAsync();

        _connectionString = _postgresContainer.GetConnectionString();

        var services = new ServiceCollection();

        services.AddDbContext<PaletteDbContext>(options =>
            options.UseNpgsql(_connectionString));

        var serviceProvider = services.BuildServiceProvider();
        _context = serviceProvider.GetRequiredService<PaletteDbContext>();

        await _context.Database.MigrateAsync();

        await _context.Palettes.AddAsync(new Palette
        {
            Title = "Test Palette",
            Author = "TestUser",
            Slug = "test-palette",
            Colors = ["#000", "#fff"],
            Tags = ["test"],
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        _client = new HttpClient(); // Optional: use if hitting your API
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task CanConnectToTestDb()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        Assert.Equal(System.Data.ConnectionState.Open, conn.State);
    }

    [Fact]
    public async Task GetAllPalettes()
    {
        var palettes = await _context.Palettes.ToListAsync();
        Assert.Single(palettes);
        Assert.Equal("Test Palette", palettes[0].Title);
        _output.WriteLine(palettes[0].Title);
        _output.WriteLine(palettes[0].Author);
        _output.WriteLine(palettes[0].Slug);
        _output.WriteLine(palettes[0].Colors[0]);
        _output.WriteLine(palettes[0].Colors[1]);
        _output.WriteLine(palettes[0].Tags[0]);
        _output.WriteLine(palettes[0].CreatedAt.ToString());
    }

    [Fact]
    public async Task ExportMarkdownPalettePreview()
    {
        var outputDir = Path.Combine(PaletteExportHelper.GetProjectRootPath(), "TestOutput");
        var outputPath = Path.Combine(outputDir, "palettes.md");

        var palettes = await _context.Palettes.ToListAsync();       
        Assert.NotEmpty(palettes);

        await PaletteExportHelper.ExportPalettesToMarkdownAsync(palettes, outputPath);

        Assert.True(File.Exists(outputPath));
        var mdPreview = await File.ReadAllTextAsync(outputPath);

        _output.WriteLine("Generated Markdown:");
        _output.WriteLine(mdPreview);
    }


}
