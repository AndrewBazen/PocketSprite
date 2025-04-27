/* PixelCanvasViewModel.cs
 *
 * @Description: This file contains the logic for the PixelCanvasViewModel class, which is the view model for the PixelCanvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @license: MIT License
 */
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp.Views.Maui;
using PocketSprite.Models;
using PocketSpriteLib.Drawing;
using System.Runtime.Versioning;

namespace PocketSprite.ViewModels
{
    /* @Class: PixelCanvasViewModel
     *
     * @Description: This class is the view model for the PixelCanvas.
     * @Author: Andrew Bazen
     * @Date: 2025-4-24
     */
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    public class PixelCanvasViewModel : ObservableObject
    {
        private PixelCanvas _pixelCanvas;
        private float _lastScaleFactor = 1;

        /* @Constructor: PixelCanvasViewModel
         *
         * @Description: Constructor for the PixelCanvasViewModel class.
         */
        public PixelCanvasViewModel()
        {
            _pixelCanvas = new PixelCanvas();
        }

        /* @Method: OnTouch
         *
         * @Description: Handles the touch event for the PixelCanvas.
         */
        public void OnTouch(object sender, SKTouchEventArgs e)
        {
            _pixelCanvas.HandleTouch(sender, e, _lastScaleFactor);
        }

        /* @Method: PaintSurface
         *
         * @Description: Handles the paint surface event for the PixelCanvas.
         */
        public void PaintSurface(SKPaintSurfaceEventArgs e)
        {
            // PaintSurface
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            var scale = DrawingUtils.CalculateScale(_pixelCanvas.Width, _pixelCanvas.Height, e.Info.Width, e.Info.Height);
            _lastScaleFactor = scale;
            canvas.Save();
            canvas.Scale(scale);
            DrawingUtils.Draw(canvas, _pixelCanvas.Width, _pixelCanvas.Height, e.Info.Width, e.Info.Height, _pixelCanvas.LayerManager.Layers);
            canvas.Restore();


        }
    }
}
