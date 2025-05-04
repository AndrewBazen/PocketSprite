using PaletteService.Models;
using System.Text;
namespace PocketSpriteTest;

public static class PaletteExportHelper
{
    public static async Task ExportPalettesToMarkdownAsync(IEnumerable<Palette> palettes, string filePath = "TestOutput/palettes.md")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        var sb = new StringBuilder();

        // YAML frontmatter
        sb.AppendLine("---");
        sb.AppendLine("title: \"🎨 Palette Preview\"");
        sb.AppendLine("tags: [palettes, test, export]");
        sb.AppendLine($"date: {DateTime.UtcNow:yyyy-MM-dd}");
        sb.AppendLine("---\n");

        sb.AppendLine("# 🎨 Palette Preview\n");
        sb.AppendLine("| Title | Author | Colors | Hex Code |");
        sb.AppendLine("|-------|--------|--------|----------|");

        foreach (var p in palettes)
        {
            var title = Escape(p.Title);
            var author = Escape(p.Author);
            var colorBadges = string.Join(" ", (p.Colors ?? []).Select(c => GenerateColorSwatch(c)));
            var color = p.Colors?[0]?.TrimStart('#') ?? "";
            var hexCode = $"![#${color}](https://via.placeholder.com/15/{color}/{color}?text=+)";

            sb.AppendLine($"| {title} | {author} | {colorBadges} | {hexCode} | `#{color}` |");
        }

        File.WriteAllText(filePath, sb.ToString());

        Console.WriteLine($"✅ Markdown file exported: {filePath}");

        await File.WriteAllTextAsync(filePath, sb.ToString());
    }

    private static string GenerateColorSwatch(string hex)
    {
        var safeHex = hex.StartsWith('#') ? hex : $"#{hex}";
        return $"<span style=\"display:inline-block;width:16px;height:16px;background-color:{safeHex};border:1px solid #ccc;\"></span> ";
    }

    private static string Escape(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";
        return text.Replace("|", "\\|");
    }

    public static string GetProjectRootPath()
    {
        // This climbs up until it finds the `.csproj` directory
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir != null && !dir.GetFiles("*.csproj").Any())
        {
            dir = dir.Parent;
        }
        dir = dir?.Parent;

        return dir?.FullName ?? throw new Exception("Project root not found.");
    }

}