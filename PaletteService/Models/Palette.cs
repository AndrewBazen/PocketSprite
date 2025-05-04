/* Palette.cs
 * 
 * @Description: This file contains the Palette class, which is used to represent a palette in the PocketSprite application.
 * @Author: Andrew Bazen
 * @Date: 2025-5-1
 * @Version: 1.0
 */
using System.ComponentModel.DataAnnotations;

namespace PaletteService.Models;

public class Palette
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Author { get; set; }

    [Required]
    public List<string> Colors { get; set; } = new();
    public List<string>? Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
