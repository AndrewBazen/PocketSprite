/* Class: LayerManager
* The LayerManager class manages multiple layers of the canvas.
* It allows adding, removing, and switching between layers.
* It also handles drawing the layers onto the canvas.
* 
* @author: Andrew Bazen
* @date: 2025-4-24
* @version: 1.0
*/
using SkiaSharp;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System;

namespace PocketSpriteLib.Drawing;


[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public class LayerManager
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
