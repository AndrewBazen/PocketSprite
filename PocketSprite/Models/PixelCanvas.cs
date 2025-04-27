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
using SkiaSharp.Views.Maui;
using PocketSpriteLib.Drawing;
using System.Runtime.Versioning;
using SkiaSharp.Views.Maui.Controls;


namespace PocketSprite.Models
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
        const int DEFAULT_PIXEL_SIZE = 6;
        private SKPoint _lastTouchPoint; // Store the last touch point
        private bool _isDrawing = false; // Flag to indicate if drawing is in progress
        public int PixelSize { get; set; } = DEFAULT_PIXEL_SIZE;
        public int Width { get; set; } = DEFAULT_WIDTH;
        public int Height { get; set; } = DEFAULT_HEIGHT;
        public LayerManager LayerManager { get; set; }

        public SKPoint LastTouchPoint // Store the last touch point
        {
            get => _lastTouchPoint;
            set
            {
                if (_lastTouchPoint != value)
                {
                    _lastTouchPoint = value;
                }
            }
        }

        /* @Constructor PixelCanvas
         *
         * @Description: Constructor for the PixelCanvas class. 
         */
        public PixelCanvas()
        {
            LayerManager = new LayerManager(Width, Height, PixelSize);
            LastTouchPoint = SKPoint.Empty;
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


        /* @Method: GetPixel
         *
         * @Description: Returns the color of a specific pixel on the canvas using x and y coordinates.
         * @param: x - The x-coordinate of the pixel.
         * @param: y - The y-coordinate of the pixel.
         * @return: The color of the pixel at the specified coordinates.
         */
        public SKColor? GetPixel(int x, int y) =>
            // Get the pixel color from the current layer
            LayerManager.CurrentLayer?.GetPixel(x, y);


        /* @Method: HandleTouch
         *
         * @Description: Handles the touch event on the canvas.
         * @param: sender - The sender of the event.
         * @param: e - The touch event arguments.
         */
        public void HandleTouch(object sender,SKTouchEventArgs e, float scale)
        {
            SKCanvasView canvasView = (SKCanvasView)sender; // Get the canvas view
            int canvasWidth = (int)canvasView.CanvasSize.Width; // Get the width of the canvas
            int canvasHeight = (int)canvasView.CanvasSize.Height; // Get the height of the canvas

            // if the touch event is released or exited, stop drawing
            if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
            {
                _isDrawing = false;
                LastTouchPoint = SKPoint.Empty;
                e.Handled = true;
                return;
            }

            // Convert touch coordinates to grid coordinates
            SKPoint gridPoint = new SKPoint(e.Location.X / scale, e.Location.Y / scale);

            // Ensure coordinates are within bounds
            if (gridPoint.X < 0 || gridPoint.X >= Width || gridPoint.Y < 0 || gridPoint.Y >= Height)
            {
                e.Handled = true;
                return;
            }

            // if the touch event is pressed, start drawing
            if (e.ActionType == SKTouchAction.Pressed)
            {
                _isDrawing = true;
                LastTouchPoint = e.Location;
                SetPixel((int)gridPoint.X, (int)gridPoint.Y, SKColors.Black); // Set the pixel to black
                e.Handled = true;                                             // TODO: update to use chosen color
            }
            else if (_isDrawing && e.ActionType == SKTouchAction.Moved)
            {
                // Convert last touch point to grid coordinates
                SKPoint lastGridPoint = new SKPoint(LastTouchPoint.X / scale, LastTouchPoint.Y / scale);

                // Draw line between points
                DrawingUtils.DrawPixelLineOnLayer(LayerManager.CurrentLayer, (int)lastGridPoint.X, (int)gridPoint.X, (int)lastGridPoint.Y, (int)gridPoint.Y, SKColors.Black);
                LastTouchPoint = e.Location;
                e.Handled = true;
            }
        }
    }
}
