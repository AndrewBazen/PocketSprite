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
    public static void DrawPixelLineOnLayer(CanvasLayer layer, int x0, int x1,
        int y0, int y1, int width, int height, int scale, SKColor color)
    { 
        // use midpoint line algorithm
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int x = x0;
        int y = y0;

      
        int d = 2 * dy - dx;
        int incE = 2 * dy;
        int incNE = 2 * (dy - dx);
        // Draw the line using the midpoint line algorithm
        int xStep = x0 < x1 ? 1 : -1;
        int yStep = y0 < y1 ? 1 : -1;

        for (int i = 0; i <= dx; i++)
        {
            if (d < 0)
            {
                d += incE;
                DrawPixel(layer, x, y, width, height, scale, color);
            }
            else
            {
                d += incNE;
                y += yStep;
                DrawPixel(layer, x, y, width, height, scale, color);
            }
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
    public static void Draw(SKCanvas canvas, int width, int height, int canvasWidth,
        int canvasHeight, int pixelSize, List<CanvasLayer> layers)
    {
        canvas.Save();
        var scaleFactor = CalculateScale(width, height, canvasWidth,
            canvasHeight, pixelSize);
        canvas.Scale(scaleFactor);

        foreach (var layer in layers)
        {
            if (layer.IsVisible)
            {
                canvas.DrawBitmap(layer.Bitmap, 0, 0);  // Draw each layer
            }
        }
        
        using var debugPaint = new SKPaint  // Draw a debug border
        { 
            Color = SKColors.Red, 
            Style = SKPaintStyle.Stroke, 
            StrokeWidth = 2 
        };
        var canvasRect = new SKRect(0, 0, width, height);
        canvas.DrawRect(canvasRect, debugPaint);
      }

    /* @Method: DrawGridOverlay
     *
     * @Description: Draws a grid overlay on the canvas.
     * @param: canvas - The SKCanvas to draw on.
     * @param: width - The width of the canvas.
     * @param: height - The height of the canvas.
     * @param: scale - The scale of the canvas.
     */
    public static void DrawGridOverlay(SKCanvas canvas,int width, int height, int pixelSize)
    {
        float gridWidth = width * pixelSize;
        float gridHeight = height * pixelSize;

        using var gridPaint = new SKPaint
        {
            Color = SKColors.Gray.WithAlpha(128), // Semi-transparent gray
            StrokeWidth = 0.5f,                   // Thin lines
            Style = SKPaintStyle.Stroke
        };

        for (int x = 0; x <= gridWidth; x += pixelSize)  // Draw vertical grid lines
        {
            canvas.DrawLine(x, 0, x, gridHeight , gridPaint);
        }

        for (int y = 0; y <= gridHeight; y += pixelSize)  // Draw horizontal grid lines
        {
            canvas.DrawLine(0, y, gridWidth, y, gridPaint);
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
}