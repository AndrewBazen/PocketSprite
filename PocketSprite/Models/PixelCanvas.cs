/* PixelCanvas.cs
 * @Description: This file contains the logic for the PixelCanvas, LayerManager, and CanvasLayer classes, which all work 
 * in tandem to draw specific pixels to the SKCanvas.
 * 
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @License: MIT License
 */
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using PocketSpriteLib.Drawing;
using System.Threading.Tasks;
using System.Runtime.Versioning;

namespace PocketSprite.Models
{
    /// PixelCanvas class
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    internal class PixelCanvas
    {
        // Constants for default canvas size and pixel size
        // These values can be adjusted based on the requirements of the application
        const int DEFAULT_PIXEL_SIZE = 6;
        const int DEFAULT_CANVAS_WIDTH = 256;
        const int DEFAULT_CANVAS_HEIGHT = 256;

        // properties for the canvas
        public int Width { get; set; } = DEFAULT_CANVAS_WIDTH;
        public int Height { get; set; } = DEFAULT_CANVAS_HEIGHT;
        public int PixelSize { get; set; } = DEFAULT_PIXEL_SIZE;
        public LayerManager LayerManager { get; set; }
        public SKCanvas MainPixelCanvas { get; set; }
        public SKBitmap MainPixelBitmap { get; set; }

        private SKPoint? _lastTouchPoint; // Store the last touch point
        private bool _isDrawing = false; // Flag to indicate if drawing is in progress

        public SKPoint? LastTouchPoint // Store the last touch point
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

        /// <summary>
        ///  Constructor for the PixelCanvas class.
        /// </summary>
        public PixelCanvas()
        {
            // Initialize the canvas and all properties
            MainPixelBitmap = new SKBitmap(Width, Height);
            MainPixelCanvas = new SKCanvas(MainPixelBitmap);
            LayerManager = new LayerManager(Width, Height, PixelSize);
            LastTouchPoint = SKPoint.Empty;

            // @TODO: setup the canvas to adjust based on the system's color theme (dark or light)
            // set the canvas color to white
            MainPixelCanvas.Clear(SKColors.White);
        }

        public async Task InitializeCanvas()
        {
            // Clear the canvas
            MainPixelCanvas.Clear(SKColors.White);

            await DrawingUtils.Draw(MainPixelCanvas, Width, Height, PixelSize, LayerManager.Layers);
        }


        /// <summary>
        /// Sets the color of a specific pixel on the canvas using x and y coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, SKColor color) =>
            // Set the pixel on the current layer
            LayerManager.CurrentLayer?.SetPixel(x, y, color);


        /// <summary>
        /// Returns the color of a specific pixel on the canvas using x and y coordinates.
        /// </summary>
        /// 
        /// <param name="x">The x-coordinate of the pixel.</param>
        /// <param name="y">The y-coordinate of the pixel.</param>
        /// <returns type="SKColor">The color of the pixel at the specified coordinates.</returns>
        public SKColor? GetPixel(int x, int y) =>
            // Get the pixel color from the current layer
            LayerManager.CurrentLayer?.GetPixel(x, y);

        /// <summary>
        /// Handles the touch event on the canvas.
        /// </summary>
        /// 
        /// <param name="sender">The sender of the touch event.</param>
        /// <param name="e">The touch event arguments.</param>
        public async Task HandleTouch(object sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.Pressed) { _isDrawing = true; }
            if (e.ActionType == SKTouchAction.Released) { _isDrawing = false; }
            if (_isDrawing && e.ActionType == SKTouchAction.Moved)
            {
                var canvasView = (SKCanvasView)sender;

                SKPoint touchPoint = e.Location;

                // Get the touch coordinates
                SKPoint convertedPoint = DrawingUtils.ConvertCoordinates(e.Location,
                    Width, Height, PixelSize, canvasView);

                // Ensure the coordinates are within bounds
                if (touchPoint.X >= 0 && touchPoint.X < Width &&
                    touchPoint.Y >= 0 && touchPoint.Y < Height)
                {
                    // Draw a line from the last point to the current point
                    if (LastTouchPoint != null)
                    {
                        // Convert the last touch point to logical grid coordinates
                        int lastX = (int)(LastTouchPoint.Value.X / PixelSize);
                        int lastY = (int)(LastTouchPoint.Value.Y / PixelSize);

                        // Convert the current touch point to logical grid coordinates
                        int currentX = (int)(convertedPoint.X / PixelSize);
                        int currentY = (int)(convertedPoint.Y / PixelSize);

                        // Draw a line between the last and current points
                        await DrawingUtils.DrawPixelLine(MainPixelCanvas, lastX, lastY, currentX, currentY, SKColors.Black);
                    }
                    else
                    {
                        // If this is the first touch, set the last touch point
                        LastTouchPoint = new SKPoint(touchPoint.X, touchPoint.Y);
                    }
                }
                else
                {
                    _lastTouchPoint = null;
                }

                e.Handled = true;

            }
            if (_isDrawing && e.ActionType != SKTouchAction.Moved)
            {
                var canvasView = (SKCanvasView)sender;

                // Get the touch coordinates
                SKPoint convertedPoint = DrawingUtils.ConvertCoordinates(e.Location,
                    Width, Height, PixelSize, canvasView);

                // Ensure the coordinates are within bounds
                if (convertedPoint.X >= 0 && convertedPoint.X < Width &&
                    convertedPoint.Y >= 0 && convertedPoint.Y < Height)
                {
                    // Set the pixel color at the touch point
                    SetPixel((int)convertedPoint.X, (int)convertedPoint.Y, SKColors.Black);
                }


            }
            // touch is released or moves outside the canvas
            else if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
            {
                // Clear the last touch point when the touch is released or moves outside the canvas
                _lastTouchPoint = null;

                e.Handled = true;
            }


        }
    }
}
