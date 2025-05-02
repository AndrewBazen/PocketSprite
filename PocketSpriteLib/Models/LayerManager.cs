/* @Class: LayerManager
 *
 * @Description: This class manages multiple layers of the canvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @Version: 1.0
 * @License: MIT
 */
using SkiaSharp;
using System.Runtime.Versioning;
using System.Collections.Generic;
using PocketSpriteLib.Drawing;

namespace PocketSpriteLib.Models;

/* @Class: LayerManager
 *
 * @Description: This class manages multiple layers of the canvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 */
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public class LayerManager
{
    // constants for defaults in pixels
    const int DEFAULT_WIDTH = 256;
    const int DEFAULT_HEIGHT = 256;
    // fields for the layer manager
    private List<CanvasLayer> _layers { get; set; } = [];
    private SKColor DefaultBackgroundColor { get; set; } = SKColors.Transparent;

    // properties for the layer manager
    public CanvasLayer CurrentLayer { get; set; }
    public int Width { get; set; } = DEFAULT_WIDTH;
    public int Height { get; set; } = DEFAULT_HEIGHT;
    public List<CanvasLayer> Layers => _layers;


    /* @Constructor: LayerManager
     *
     * @Description: Constructor for the LayerManager class.
     * @param: width - The width of the canvas.
     * @param: height - The height of the canvas.
     * @param: pixelSize - The size of the pixels.
     */
    public LayerManager()
    {
        
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


    /* @Method: AddLayer
     *
     * @Description: Adds a new layer to the canvas.
     * @param: backgroundColor - The color of the layer.
     */
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

    /* @Method: RemoveLayer
     *
     * @Description: Removes a layer from the canvas.
     * @param: index - The index of the layer to remove.
     */
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

    /* @Method: SwitchToLayer
     *
     * @Description: Switches to a layer.
     * @param: index - The index of the layer to switch to.
     */
    public void SwitchToLayer(int index)
    {
        if (index >= 0 && index < Layers.Count)
        {
            CurrentLayer = Layers[index];
        }
    }

}
