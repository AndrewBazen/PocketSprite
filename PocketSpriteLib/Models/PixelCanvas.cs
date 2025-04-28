/* PixelCanvas.cs
 *
 * @Description: This file contains the logic for the PixelCanvas, LayerManager, and CanvasLayer classes, which all work 
 * in tandem to draw specific pixels to the SKCanvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @license: MIT License 
 */
using SkiaSharp;
using System.Runtime.Versioning;
using PocketSpriteLib.Drawing;


namespace PocketSpriteLib.Models
{
    /* @Class PixelCanvas
     *
     * @Description: This class contains the logic for the PixelCanvas, LayerManager, and CanvasLayer classes, which all work 
     * in tandem to draw specific pixels to the SKCanvas.
     * @Author: Andrew Bazen
     * @Date: 2025-4-24 
     */
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    public class PixelCanvas
    {
        // constants for defaults in pixels
        const int DEFAULT_WIDTH = 256;
        const int DEFAULT_HEIGHT = 256;
        public int Width { get; set; } = DEFAULT_WIDTH;
        public int Height { get; set; } = DEFAULT_HEIGHT;
        public LayerManager LayerManager { get; set; }

       
        /* @Constructor PixelCanvas
         *
         * @Description: Constructor for the PixelCanvas class. 
         */
        public PixelCanvas()
        {
            LayerManager = new LayerManager(Width, Height);
        }


        /* @Method: SetPixel
         *
         * @Description: Sets the pixel on the current layer.
         * @param: x - The x-coordinate of the pixel.
         * @param: y - The y-coordinate of the pixel.
         * @param: color - The color of the pixel.
         */
        public void SetPixel(int x, int y, SKColor color) =>
            // Set the pixel on the current layer
            LayerManager.CurrentLayer?.SetPixel(x, y, color);


        public SKColor GetPixel(int x, int y)
        {
            // Get the pixel color from the current layer
            return LayerManager.CurrentLayer?.GetPixel(x, y) ?? SKColors.Transparent;
        }

        public void DrawPixelLine(int x0, int x1, int y0, int y1, SKColor color)
        {
            // Draw a line between two points
            DrawingUtils.DrawPixelLineOnLayer(LayerManager.CurrentLayer, x0, x1, y0, y1, color);
        }
    }
}
