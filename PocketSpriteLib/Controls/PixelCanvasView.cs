using SkiaSharp.Views.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Runtime.Versioning;
using PocketSpriteLib.Models;
using PocketSpriteLib.Drawing;

namespace PocketSpriteLib.Controls
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    public class PixelCanvasView : SKCanvasView
    {
        private PixelCanvas _pixelCanvas;
        private float _scaleFactor = 1;
        private SKPoint _lastTouchPoint;
        private bool _isDrawing = false; // Flag to indicate if drawing is in progress
        public PixelCanvasView()
        {
            _pixelCanvas = new PixelCanvas();

            // Initialize the canvas view
            EnableTouchEvents = true;
            PaintSurface += OnPaintSurface;
            Touch += OnTouch;

        }
        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            float logicalWidth = _pixelCanvas.Width;
            float logicalHeight = _pixelCanvas.Height;
            _scaleFactor = DrawingUtils.CalculateScale(logicalWidth, logicalHeight, e.Info.Width, e.Info.Height);

            canvas.Save();
            canvas.Scale(_scaleFactor);

            DrawingUtils.Draw(canvas, _pixelCanvas.Width, _pixelCanvas.Height, e.Info.Width, e.Info.Height, _pixelCanvas.LayerManager.Layers);

            canvas.Restore();
        }
        private void OnTouch(object? sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
            {
                _isDrawing = false;
                _lastTouchPoint = SKPoint.Empty;
                e.Handled = false;
                return;
            }

            var adjustedPoint = new SKPoint(e.Location.X / _scaleFactor, e.Location.Y / _scaleFactor);
            int gridX = (int)adjustedPoint.X;
            int gridY = (int)adjustedPoint.Y;
            if (gridX < 0 || gridX >= _pixelCanvas.Width || gridY < 0 || gridY >= _pixelCanvas.Height)
            {
                e.Handled = false;
                return;
            }

            if (e.ActionType == SKTouchAction.Pressed)
            {
                // Start drawing
                _isDrawing = true;
                _lastTouchPoint = adjustedPoint;
                _pixelCanvas.SetPixel((int)Math.Round(adjustedPoint.X), (int)Math.Round(adjustedPoint.Y), SKColors.Black);
                InvalidateSurface();
                e.Handled = true;
            }
            else if (_isDrawing && e.ActionType == SKTouchAction.Moved)
            {
                int lastGridx = (int)_lastTouchPoint.X;
                int lastGridy = (int)_lastTouchPoint.Y;
                _pixelCanvas.DrawPixelLine(lastGridx, gridX, lastGridy, gridY, SKColors.Black);
                _lastTouchPoint = adjustedPoint;
                
            }

            InvalidateSurface();
            e.Handled = true;
        }
    }
}
