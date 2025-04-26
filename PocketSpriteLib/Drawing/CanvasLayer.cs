/* * File: CanvasLayer.cs
 * @Description: This file contains the logic for the CanvasLayer class, 
 * which represents a single layer of the canvas.
 * 
 * 
 * @author: Andrew Bazen
 * @date: 2025-4-24
 * @version: 1.0
 */

using System.Runtime.Versioning;
using SkiaSharp;


namespace PocketSpriteLib.Drawing;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public class CanvasLayer(int width, int height, SKColor backgroundColor, float opacity = 1.0f, bool isVisible = true)
{
    public SKBitmap Bitmap { get; private set; } = new SKBitmap(width, height);
    public bool IsVisible { get; set; } = isVisible;
    public float Opacity { get; set; } = opacity; // 0.0 (transparent) to 1.0 (opaque)

    public SKColor BackgroundColor { get; private set; } = backgroundColor;
    public void InitializeNewLayer()
    {
        using var canvas = new SKCanvas(Bitmap);
        canvas.Clear(BackgroundColor);
    }

    public void SetPixel(int x, int y, SKColor color)
    {
        if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
        {
            Bitmap.SetPixel(x, y, color);
        }
    }

    public SKColor GetPixel(int x, int y)
    {

        // Check if the coordinates are within the bitmap bounds
        if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
        {
            return Bitmap.GetPixel(x, y);
        }
        return SKColors.Transparent; // Return transparent if out of bounds
    }
}
