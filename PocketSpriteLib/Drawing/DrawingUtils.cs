/* DrawingUtils.cs
 *
 * @Description: This file contains the logic for the DrawingUtils class, which provides utility methods for drawing on a canvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @license: MIT License
 */
using PocketSpriteLib.Models;
using SkiaSharp;
using System.Runtime.Versioning;


namespace PocketSpriteLib.Drawing;

/* @Class: DrawingUtils
 *
 * @Description: This class provides utility methods for drawing on a canvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 */
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public class DrawingUtils
{

    public static void DrawPixel(CanvasLayer layer, int x, int y, int width,
        int height, int scale, SKColor color)
    {
        //colors all pixels in the same scaled region
        int x1 = x - (scale / 2);
        int x2 = x + (scale / 2);
        int y1 = y - (scale / 2);
        int y2 = y + (scale / 2);

        if (x1 < 0) x1 = 0;
        if (x2 >= width) x2 = width - 1;
        if (y1 < 0) y1 = 0;
        if (y2 >= height) y2 = height - 1;
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                layer.Bitmap.SetPixel(i, j, color);
            }
        }
    }

    /* @Method: DrawPixelLineOnLayer
     *
     * @Description: Draws a line on a canvas layer.
     * @param: layer - The layer to draw on.
     * @param: x0 - The starting x coordinate.
     * @param: x1 - The ending x coordinate.
     * @param: y0 - The starting y coordinate.
     * @param: y1 - The ending y coordinate.
     * @param: color - The color of the line.
     */
    public static void DrawPixelLineOnLayer(
       PixelCanvas canvas,
       int x0, int x1, int y0, int y1,
       SKColor color) {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int x = x0;
        int y = y0;

        int xStep = (x0 < x1) ? 1 : -1;
        int yStep = (y0 < y1) ? 1 : -1;

        bool steep = dy > dx;

        if (steep) {
            // Swap x and y for steep lines
            (x, y) = (y, x);
            (dx, dy) = (dy, dx);
            (xStep, yStep) = (yStep, xStep);
            (x0, y0) = (y0, x0);
        }

        int d = 2 * dy - dx;

        for (int i = 0; i <= dx; i++) {
            if (steep)
                canvas.SetPixel(y, x, color);  // y and x are swapped back
            else
                canvas.SetPixel(x, y, color);

            if (d > 0) {
                y += yStep;
                d -= 2 * dx;
            }

            d += 2 * dy;
            x += xStep;
        }
    }


    /* @Method: Draw
     *
     * @Description: Draws the layers on the canvas.
     * @param: canvas - The SKCanvas to draw on.
     * @param: width - The width of the canvas.
     * @param: height - The height of the canvas.
     * @param: canvasWidth - The width of the canvas.
     * @param: canvasHeight - The height of the canvas.
     * @param: layers - The layers to draw.
     */
    public static void Draw(SKCanvas canvas, int logicalWidth, int logicalHeight, int viewWidth,
        int viewHeight, int pixelSize, List<CanvasLayer> layers)
    {
        canvas.Save();
        
        float scaledWidth = logicalWidth * pixelSize;
        float scaledHeight = logicalHeight * pixelSize;

        float offsetX = (viewWidth - scaledWidth) / 2f;
        float offsetY = (viewHeight - scaledHeight) / 2f;

        foreach (var layer in layers)
        {
            if (layer.IsVisible)
            {
                DrawLayerPixelPerfect(canvas, layer, pixelSize);
            }
        }
        
        using var debugPaint = new SKPaint  // Draw a debug border
        { 
            Color = SKColors.Red, 
            Style = SKPaintStyle.Stroke, 
            StrokeWidth = 2 
        };
        var canvasRect = new SKRect(0, 0, logicalWidth * pixelSize, logicalHeight * pixelSize);
        canvas.DrawRect(canvasRect, debugPaint);

        canvas.Restore();
      }

    /* @Method: DrawGridOverlay
     *
     * @Description: Draws a grid overlay on the canvas.
     * @param: canvas - The SKCanvas to draw on.
     * @param: width - The width of the canvas.
     * @param: height - The height of the canvas.
     * @param: scale - The scale of the canvas.
     */
    public static void DrawGridOverlay(SKCanvas canvas, float offsetX, float offsetY,
        int logicalWidth, int logicalHeight, float Zoom,  int pixelSize)
    {
        using var gridPaint = new SKPaint {
            Color = SKColors.Gray.WithAlpha(100),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            IsAntialias = false
        };

        // Draw grid in logical pixel space
        for (int x = 0; x <= logicalWidth; x++) {
            float gx = x * pixelSize;
            canvas.DrawLine(gx, 0, gx, logicalHeight * pixelSize, gridPaint);
        }
        for (int y = 0; y <= logicalHeight; y++) {
            float gy = y * pixelSize;
            canvas.DrawLine(0, gy, logicalWidth * pixelSize, gy, gridPaint);
        }
    }

    /* @Method: CalculateScale
     *
     * @Description: Calculates the scale factor for the grid.
     * @param: logicalWidth - The width of the logical grid.
     * @param: logicalHeight - The height of the logical grid.
     * @param: canvasWidth - The width of the canvas.
     * @param: canvasHeight - The height of the canvas.
     * @return: The scale factor.
     */
    public static float CalculateScale(float logicalWidth, float logicalHeight, float canvasWidth,
        float canvasHeight, int pixelSize)
    {
        float gridWidth = logicalWidth * pixelSize;
        float gridHeight = logicalHeight * pixelSize;

        float scaleX = canvasWidth / gridWidth;
        float scaleY = canvasHeight / gridHeight;


        return Math.Min(scaleX, scaleY);
    }

    public static void DrawCheckerboard(SKCanvas canvas, int width, int height, int cellSize) {
        using var lightPaint = new SKPaint { Color = new SKColor(200, 200, 200) };
        using var darkPaint = new SKPaint { Color = new SKColor(160, 160, 160) };

        cellSize = Math.Clamp(cellSize, 8, 256);
        for (int y = 0; y < height; y += cellSize) {
            for (int x = 0; x < width; x += cellSize) {
               
                bool isLight = ((x / cellSize) + (y / cellSize)) % 2 == 0;
                var paint = isLight ? lightPaint : darkPaint;
                canvas.DrawRect(x, y, cellSize, cellSize, paint);
            }
        }
    }

    public static void DrawLayerPixelPerfect(SKCanvas canvas, CanvasLayer layer, int pixelSize) {
        if (!layer.IsVisible) return;

        int width = layer.Bitmap.Width;
        int height = layer.Bitmap.Height;

        var sourceRect = new SKRect(0, 0, width, height);
        var destRect = new SKRect(0, 0, width * pixelSize, height * pixelSize);

        using var paint = new SKPaint {
            FilterQuality = SKFilterQuality.None, // this is deprecated but still works on older versions
            IsAntialias = false,
            IsDither = false
        };

        canvas.DrawBitmap(layer.Bitmap, sourceRect, destRect, paint);
    }


}