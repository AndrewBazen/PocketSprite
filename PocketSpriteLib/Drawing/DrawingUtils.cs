using System;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using System.Collections.Generic;

namespace PocketSpriteLib.Drawing;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public class DrawingUtils
{
    public static async Task DrawPixel(SKCanvas canvas, int x, int y, SKColor color)
    {
        using var paint = new SKPaint
        {
            Color = color,
            Style = SKPaintStyle.Fill
        };
        await Task.Run(() => canvas.DrawPoint(x, y, paint));
    }
    public static async Task DrawPixelLine(SKCanvas canvas, int x0, int x1, int y0, int y1, SKColor color)
    {
        // use midpoint line algorithm
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int x = x0;
        int y = y0;

        bool steep = dy > dx;
        if (steep)
        {
            // swap dx and dy
            int temp = dx;
            dx = dy;
            dy = temp;
        }

        int d = (2 * dy) - dx;
        int incE = 2 * dy;
        int incNE = 2 * (dy - dx);
        // Draw the line using the midpoint line algorithm
        int xStep = (x0 < x1) ? 1 : -1;
        int yStep = (y0 < y1) ? 1 : -1;

        await Task.Run(async () =>
        {
            for (int i = 0; i <= dx; i++)
            {
                if (steep)
                {
                    await DrawPixel(canvas, y, x, color);
                }
                else
                {
                    await DrawPixel(canvas, x, y, color);
                }

                if (d < 0)
                {
                    d += incE;
                }
                else
                {
                    d += incNE;
                    y += yStep;
                }
                x += xStep;
            }

        });
    }

    public static async Task Draw(SKCanvas canvas, int width, int height, int pixelSize, List<CanvasLayer> layers)
    {
        // get the canvas size
        float canvasWidth = canvas.DeviceClipBounds.Width;
        float canvasHeight = canvas.DeviceClipBounds.Height;

        // Calculate rendered grid size
        float gridWidth = width * pixelSize;
        float gridHeight = height * pixelSize;

        // Calculate scale factor
        float scaleX = canvasWidth / gridWidth;
        float scaleY = canvasHeight / gridHeight;

        // Use the smaller scale factor to fit the grid within the canvas
        float scale = Math.Min(scaleX, scaleY);

        canvas.Scale(scale);

        await Task.Run(() =>
        {
            // Draw the grid overlay
            DrawGridOverlay(canvas, width, height, pixelSize);
            foreach (CanvasLayer layer in layers)
            {
                if (layer.IsVisible)
                {
                    float renderedWidth = layer.Bitmap.Width * pixelSize;
                    float renderedHeight = layer.Bitmap.Height * pixelSize;
                    Console.WriteLine($"Rendered Grid Size: {renderedWidth}x{renderedHeight}");
                    using var paint = new SKPaint
                    {
                        Color = SKColors.White.WithAlpha((byte)(layer.Opacity * 255))
                    };

                    // Scale the bitmap
                    var destRect = new SKRect(0, 0, layer.Bitmap.Width * pixelSize, layer.Bitmap.Height * pixelSize);
                    canvas.DrawBitmap(layer.Bitmap, destRect, paint);

                    // Debug rectangle
                    using var debugPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
                    canvas.DrawRect(destRect, debugPaint); // Draw a red border around the grid
                }
            }
        });

    }

    /// <summary>
    /// Draws a grid overlay on the canvas.
    /// </summary>
    /// 
    /// <param name="canvas">The SKCanvas to draw on.</param>
    /// <param name="width">The width of the canvas.</param>
    /// <param name="height">The height of the canvas.</param>
    /// <param name="pixelSize">The size of each pixel in the grid.</param>
    public static void DrawGridOverlay(SKCanvas canvas, int width, int height, int pixelSize)
    {
        float gridWidth = width * pixelSize;
        float gridHeight = height * pixelSize;

        // // Calculate the scale factor applied during rendering
        // float scaleX = width / gridWidth;
        // float scaleY = height / gridHeight;
        // float scale = Math.Min(scaleX, scaleY);

        // // Draw the grid overlay using the scaled pixel size
        // int scaledPixelSize = (int)(pixelSize * scale);

        using var gridPaint = new SKPaint
        {
            Color = SKColors.Gray.WithAlpha(128), // Semi-transparent gray
            StrokeWidth = 0.5f,                     // Thin lines
            Style = SKPaintStyle.Stroke
        };

        // Draw vertical grid lines
        for (int x = 0; x <= gridWidth; x += pixelSize)
        {
            canvas.DrawLine(x, 0, x, gridHeight, gridPaint);
        }

        // Draw horizontal grid lines
        for (int y = 0; y <= gridHeight; y += pixelSize)
        {
            canvas.DrawLine(0, y, gridWidth, y, gridPaint);
        }
    }

    public static SKPoint ConvertCoordinates(SKPoint touchPoint, int width,
        int height, int pixelSize, SKCanvasView canvasView)
    {
        // Get the canvas scale (use the same logic as in Draw)
        float canvasWidth = canvasView.CanvasSize.Width;
        float canvasHeight = canvasView.CanvasSize.Height;

        float gridWidth = width * pixelSize;
        float gridHeight = height * pixelSize;
        // Calculate the scale factor applied during rendering
        float scaleX = canvasWidth / gridWidth;
        float scaleY = canvasHeight / gridHeight;
        float scale = Math.Min(scaleX, scaleY);
        // Adjust touch coordinates to match the scaled grid
        float adjustedX = touchPoint.X / scale;
        float adjustedY = touchPoint.Y / scale;
        return new SKPoint(adjustedX, adjustedY);
    }
}