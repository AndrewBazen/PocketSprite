/* CanvasLayer.cs
 *
 * @Description: This file contains the logic for the CanvasLayer class, 
 * which represents a single layer of the canvas.
 * @author: Andrew Bazen
 * @date: 2025-4-24
 * @version: 1.0
 * @license: MIT License
 */
using System.Runtime.Versioning;
using SkiaSharp;

namespace PocketSpriteLib.Drawing;

/* @Class/@Constructor: CanvasLayer
 *
 * @Description: This class represents a single layer of the canvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @param: width - The width of the layer.
 * @param: height - The height of the layer.
 * @param: backgroundColor - The color of the layer.
 * @param: opacity - The opacity of the layer.
 * @param: isVisible - Whether the layer is visible.
 */
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

    /* @Method: InitializeNewLayer
     *
     * @Description: Initializes a new layer.
     */
    public void InitializeNewLayer()
    {
        using var canvas = new SKCanvas(Bitmap);
        canvas.Clear(BackgroundColor);
    }

    /* @Method: SetPixel
     *
     * @Description: Sets the pixel at the specified coordinates.
     * @param: x - The x-coordinate of the pixel.
     * @param: y - The y-coordinate of the pixel.
     * @param: color - The color of the pixel.
     */
    public void SetPixel(int x, int y, SKColor color)
    {
        if (x >= 0 && x < Bitmap.Width && y >= 0 && y < Bitmap.Height)
        {
            Bitmap.SetPixel(x, y, color);
        }
    }

    /* @Method: GetPixel
     *
     * @Description: Gets the pixel at the specified coordinates.
     * @param: x - The x-coordinate of the pixel.
     * @param: y - The y-coordinate of the pixel.
     * @return: The color of the pixel at the specified coordinates.
     */
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
