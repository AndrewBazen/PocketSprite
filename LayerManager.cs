using System;
using SkiaSharp;

namespace PocketSprite;

public class LayerManager
{
    private readonly List<CanvasLayer> layers = new();

    public CanvasLayer currentLayer { get; private set; }

    public int _width { get; }
    public int _height { get; }
    public int _pixelSize { get; }

    private SKColor defaultBackgroundColor = SKColors.White;

    public LayerManager(int width, int height, int pixelSize)
 
    {
        _width = width;
        _height = height;
        _pixelSize = pixelSize;

        // Add background layer
        var backgroundLayer = new CanvasLayer(width, height, defaultBackgroundColor);
        layers.Add(backgroundLayer);

        // Add default drawing layer
        var drawingLayer = new CanvasLayer(width, height, SKColors.Transparent);
        layers.Add(drawingLayer);

        currentLayer = drawingLayer;
    }

    public void AddLayer(SKColor backgroundColor = default)
    {
        var newLayer = new CanvasLayer(_width, _height, backgroundColor);
        layers.Add(newLayer);
    }

    public void RemoveLayer(int index)
    {
        if (index > 0 && index < layers.Count) // Prevent removing the background layer
        {
            layers.RemoveAt(index);
        }
    }

    public void SwitchToLayer(int index)
    {
        if (index >= 0 && index < layers.Count)
        {
            currentLayer = layers[index];
        }
    }

    public void Draw(SKCanvas canvas)
    {
        float canvasWidth = canvas.DeviceClipBounds.Width;
        float canvasHeight = canvas.DeviceClipBounds.Height;

        // Calculate rendered grid size
        float gridWidth = _width * _pixelSize;
        float gridHeight = _height * _pixelSize;

        // Calculate scale factor
        float scaleX = canvasWidth / gridWidth;
        float scaleY = canvasHeight / gridHeight;

        // Use the smaller scale factor to fit the grid within the canvas
        float scale = Math.Min(scaleX, scaleY);

        canvas.Scale(scale);

        foreach (var layer in layers)
        {
            if (layer.isVisible)
            {
                float renderedWidth = layer._bitmap.Width * _pixelSize;
                float renderedHeight = layer._bitmap.Height * _pixelSize;
                Console.WriteLine($"Rendered Grid Size: {renderedWidth}x{renderedHeight}");
                layer.Draw(canvas, _pixelSize);
            }
        }
    }
}
