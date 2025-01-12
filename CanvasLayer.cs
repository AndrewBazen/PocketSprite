using SkiaSharp;

namespace PocketSprite;

public class CanvasLayer
{
    public SKBitmap _bitmap { get; }
    public bool isVisible { get; set; } = true;
    public float opacity { get; set; } = 1.0f; // 0.0 (transparent) to 1.0 (opaque)

    public CanvasLayer(int width, int height, SKColor backgroundColor)
    {
        _bitmap = new SKBitmap(width, height);

        // Initialize the layer with the background color
        using var canvas = new SKCanvas(_bitmap);
        canvas.Clear(backgroundColor);
    }

    public void SetPixel(int x, int y, SKColor color)
    {
        if (x >= 0 && x < _bitmap.Width && y >= 0 && y < _bitmap.Height)
        {
            _bitmap.SetPixel(x, y, color);
        }
    }

    public void Draw(SKCanvas canvas, int pixelSize)
    {
        if (!isVisible) return;
        using var paint = new SKPaint
        {
            Color = SKColors.White.WithAlpha((byte)(opacity * 255))
        };

        // Scale the bitmap
        var destRect = new SKRect(0, 0, _bitmap.Width * pixelSize, _bitmap.Height * pixelSize);
        canvas.DrawBitmap(_bitmap, destRect, paint);

        // Debug rectangle
        using var debugPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
        canvas.DrawRect(destRect, debugPaint); // Draw a red border around the grid
    }
}

