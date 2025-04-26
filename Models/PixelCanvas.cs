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
using SkiaSharp.Views.Maui.Controls;

namespace PocketSprite.Models
{

    /* Class: PixelCanvas
     * The PixelCanvas class represents a pixel canvas for drawing.
     * 
     */
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


        /* Constructor: PixelCanvas
         * initializes a new instance of the PixelCanvas class with the specified width, height, and pixel size.
         * 
         * @param width: The width of the canvas in pixels.
         * @param height: The height of the canvas in pixels.
         * @param pixelSize: The size of each pixel in the canvas.
         */
        public PixelCanvas()
        {
            // Initialize the canvas and all properties
            MainPixelBitmap = new SKBitmap(Width, Height);
            MainPixelCanvas = new SKCanvas(MainPixelBitmap);
            LayerManager = new LayerManager(Width, Height, PixelSize);

            // @TODO: setup the canvas to adjust based on the system's color theme (dark or light)
            // set the canvas color to white
            MainPixelCanvas.Clear(SKColors.White);
        }


        /* Method: SetPixel
         * Sets the color of a specific pixel on the canvas using x and y coordinates.
         * 
         * @param x: The x-coordinate of the pixel.
         * @param y: The y-coordinate of the pixel.
         * @param color: The color to set the pixel to.
         */
        public void SetPixel(int x, int y, SKColor color) =>
            // Set the pixel on the current layer
            LayerManager.CurrentLayer?.SetPixel(x, y, color);


        /* Method: GetPixel
         * Returns the color of a specific pixel on the canvas using x and y coordinates.
         * 
         * @param x: The x-coordinate of the pixel.
         * @param y: The y-coordinate of the pixel.
         * @return: The color of the pixel at the specified coordinates.
         */
        public SKColor? GetPixel(int x, int y) =>
            // Get the pixel color from the current layer
            LayerManager.CurrentLayer?.GetPixel(x, y);


        /* Method: DrawGridOverlay
         * Returns the color of a specific pixel on the canvas using x and y coordinates.
         * 
         * @param x: The x-coordinate of the pixel.
         * @param y: The y-coordinate of the pixel.
         * @return: The color of the pixel at the specified coordinates.
         */
        public void DrawGridOverlay(SKCanvas canvas, int canvasWidth, int canvasHeight)
        {
            float gridWidth = LayerManager.Width * PixelSize;
            float gridHeight = LayerManager.Height * PixelSize;

            // Calculate the scale factor applied during rendering
            float scaleX = canvasWidth / gridWidth;
            float scaleY = canvasHeight / gridHeight;
            float scale = Math.Min(scaleX, scaleY);

            // Draw the grid overlay using the scaled pixel size
            int scaledPixelSize = (int)(PixelSize * scale);

            using var gridPaint = new SKPaint
            {
                Color = SKColors.Gray.WithAlpha(128), // Semi-transparent gray
                StrokeWidth = 0.5f,                     // Thin lines
                Style = SKPaintStyle.Stroke
            };

            // Draw vertical grid lines
            for (int x = 0; x <= gridWidth; x += PixelSize)
            {
                canvas.DrawLine(x, 0, x, gridHeight, gridPaint);
            }

            // Draw horizontal grid lines
            for (int y = 0; y <= gridHeight; y += PixelSize)
            {
                canvas.DrawLine(0, y, gridWidth, y, gridPaint);
            }
        }

        public void Draw(SKCanvas canvas, int width, int height, int pixelSize, List<CanvasLayer> layers)
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
        }

        public SKPoint ConvertCoordinates(SKPoint touchPoint, SKCanvasView canvasView)
        {
            // Get the canvas scale (use the same logic as in Draw)
            float canvasWidth = canvasView.CanvasSize.Width;
            float canvasHeight = canvasView.CanvasSize.Height;

            float gridWidth = LayerManager.Width * PixelSize;
            float gridHeight = LayerManager.Height * PixelSize;
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

    /*
     * The CanvasLayer class represents a single layer of the canvas.
     * 
     * @author: Andrew Bazen
     * @date: 2025-4-24
     */
    public class CanvasLayer(int width, int height, SKColor backgroundColor, float opacity = 1.0f, bool isVisible = true)
    {
        public SKBitmap Bitmap { get; private set; } = new SKBitmap(width, height);
        public bool IsVisible { get; set; } = isVisible;
        public float Opacity { get; set; } = opacity; // 0.0 (transparent) to 1.0 (opaque)

        public SKColor BackgroundColor { get; private set; } = backgroundColor;

        public int Height => height;

        public int Width => width;

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

    /* Class: LayerManager
     * The LayerManager class manages multiple layers of the canvas.
     * It allows adding, removing, and switching between layers.
     * It also handles drawing the layers onto the canvas.
     * 
     * @author: Andrew Bazen
     * @date: 2025-4-24
     */
    internal class LayerManager
    {
        // fields for the layer manager
        private List<CanvasLayer> _layers { get; set; } = [];
        private SKColor DefaultBackgroundColor { get; set; } = SKColors.White;

        // properties for the layer manager
        public CanvasLayer? CurrentLayer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int PixelSize { get; set; }
        public List<CanvasLayer> Layers => _layers;



        public LayerManager(int width, int height, int pixelSize)
        {
            Width = width;
            Height = height;
            PixelSize = pixelSize;

            // Add background layer
            var backgroundLayer = new CanvasLayer(Width, Height, DefaultBackgroundColor);
            backgroundLayer.InitializeNewLayer();
            Layers.Add(backgroundLayer);

            // Add default drawing layer
            var drawingLayer = new CanvasLayer(Width, Height, DefaultBackgroundColor);
            drawingLayer.InitializeNewLayer();
            Layers.Add(drawingLayer);

            CurrentLayer = drawingLayer;
        }


        public void AddLayer(SKColor backgroundColor = default)
        {

            if (backgroundColor != default)
            {
                var newLayer = new CanvasLayer(Width, Height, backgroundColor);
                newLayer.InitializeNewLayer();
                Layers.Add(newLayer);
            }
            else
            {
                var newLayer = new CanvasLayer(Width, Height, SKColors.Transparent);
                newLayer.InitializeNewLayer();
                Layers.Add(newLayer);
            }
        }

        public void RemoveLayer(int index)
        {
            if (index > 0 && index < Layers.Count) // Prevent removing the background layer
            {
                // Remove the layer at the specified index

                // delete the layer from the view
                if (Layers.Count > 1)
                {
                    CurrentLayer = Layers[Layers.Count - 1]; // Switch to the last layer
                    Layers.RemoveAt(index);
                }
                else
                {
                    throw new InvalidOperationException("Cannot remove the Background layer");
                }
            }
        }

        public void SwitchToLayer(int index)
        {
            if (index >= 0 && index < Layers.Count)
            {
                CurrentLayer = Layers[index];
            }
        }
    }
}
