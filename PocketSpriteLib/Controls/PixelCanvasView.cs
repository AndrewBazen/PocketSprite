using SkiaSharp.Views.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Runtime.Versioning;
using PocketSpriteLib.Models;
using PocketSpriteLib.Drawing;
using System.Diagnostics;
using Microsoft.Maui.Platform;

namespace PocketSpriteLib.Controls
{
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("android")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    public class PixelCanvasView : SKCanvasView
    {
        private float Zoom = 1.0f;
        private float PrevZoom = 1.0f;
        private float MIN_ZOOM = 0.5f;
        private float MAX_ZOOM = 8.0f;
        private SKPoint PanOffset = new(0, 0);
        private SKPoint PrevPanOffset = new(0, 0);
        private const double OVERSHOOT = 0.2f;
        private double StartScale, LastScale;

        private PixelCanvas _pixelCanvas;
        private int pixelSize; // Default pixel size
        private SKPoint _lastTouchPoint;
        private bool _isDrawing = false; // Flag to indicate if drawing is in progress
        public PixelCanvasView()
        {
            _pixelCanvas = new PixelCanvas();

            //TODO: Add pinch gesture recognizer for zooming on touch screen
            //var pinch = new PinchGestureRecognizer();
            //pinch.PinchUpdated += OnPinchUpdated;
            //GestureRecognizers.Add(pinch);

            // TODO: Add pan gesture recognizer for panning on touch screen

            // TODO: Add tap gesture recognizer for selecting pixels on touch screen

            


            // Initialize the canvas view
            EnableTouchEvents = true;
            PaintSurface += OnPaintSurface;
            Touch += OnTouch;
            Touch += OnMouseWheel;

            // Set the initial translation and anchor point
            AnchorX = AnchorY = 0;
            TranslationX = TranslationY = 0;

        }
        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            // Get the canvas and clear it
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            // Calculate the scale based on the canvas size and pixel canvas size
            var scaleWidth = e.Info.Width / _pixelCanvas.Width;
            var scaleHeight = e.Info.Height / _pixelCanvas.Height;
            pixelSize = Math.Min(scaleWidth, scaleHeight); // scale is donoted as pixel size

            canvas.Save(); // save the current canvas state

            ZoomAndTranslate(canvas); // apply any changes to the translation and zoom
                    
            float logicalWidth = _pixelCanvas.Width; // logical width is the true width of the pixel canvas
            float logicalHeight = _pixelCanvas.Height; // logical height is the true height of the pixel canvas

            // draw the pixel canvas first and then the grid overlay
            DrawingUtils.Draw(canvas, (int)logicalWidth, (int)logicalHeight, canvas.DeviceClipBounds.Width, canvas.DeviceClipBounds.Height, pixelSize, _pixelCanvas.LayerManager.Layers);
            DrawingUtils.DrawGridOverlay(canvas, canvas.DeviceClipBounds.Width, canvas.DeviceClipBounds.Height, pixelSize);

            canvas.Restore(); // restore the canvas state
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

            var adjustedPoint = new SKPoint(e.Location.X / pixelSize, e.Location.Y / pixelSize);
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
                int lastGridx = (int)Math.Round(_lastTouchPoint.X);
                int lastGridy = (int)Math.Round(_lastTouchPoint.Y);
                _pixelCanvas.DrawPixelLine(lastGridx, gridX, lastGridy, gridY, _pixelCanvas.Width,
                    _pixelCanvas.Height, pixelSize, SKColors.Black);
                _lastTouchPoint = adjustedPoint;

            }

            InvalidateSurface();
            e.Handled = true;
        }
        private void OnMouseWheel(object? sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.WheelChanged)
            {
                // Get the size of the canvas
                float canvasWidth = this.CanvasSize.Width;
                float canvasHeight = this.CanvasSize.Height;

                // Calculate the zoom factor based on the mouse wheel delta
                float zoomDelta = e.WheelDelta > 0 ? 1.1f : 0.9f;
                float newZoom = Zoom * zoomDelta;
                float clampedZoom = Math.Clamp(newZoom, MIN_ZOOM, MAX_ZOOM); // Clamp zoom to bounds

                PrevPanOffset = PanOffset;
                PrevZoom = Zoom;
                Zoom = newZoom;

                if (e.WheelDelta > 0)
                {
                    // Zooming in → focus on cursor
                    var mousePos = new SKPoint(e.Location.X / PrevZoom, e.Location.Y / PrevZoom);
                    PanOffset.X = mousePos.X - (mousePos.X - PanOffset.X) * (Zoom / PrevZoom);
                    PanOffset.Y = mousePos.Y - (mousePos.Y - PanOffset.Y) * (Zoom / PrevZoom);
                }
                else
                {
                    // Zooming out → center of view is anchor
                    var center = new SKPoint(canvasWidth / 2f / PrevZoom, canvasHeight / 2f / PrevZoom);
                    PanOffset.X = center.X - (center.X - PanOffset.X) * (Zoom / PrevZoom);
                    PanOffset.Y = center.Y - (center.Y - PanOffset.Y) * (Zoom / PrevZoom);
                }
                // If zoom is out of bounds, only animate Zoom back — do not touch PanOffset
                if (Zoom != clampedZoom)
                {
                    AnimateZoomToBounds(clampedZoom); // ← only adjusts Zoom, not PanOffset
                }


                Debug.WriteLine($"Zoom: {Zoom}, Pan: {PanOffset}");

                InvalidateSurface();
            }

            e.Handled = true;
        }

        private void AnimateZoomToBounds(float targetZoom)
        {
            new Animation(
                (value) =>
                {
                    Zoom = (float)value;
                    InvalidateSurface(); // Redraw every animation frame
                },
                Zoom,
                targetZoom
            ).Commit(this, "ZoomBounce", easing: Easing.SpringOut);
        }


        //private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
        //{
        //    switch (e.Status)
        //    {
        //        case GestureStatus.Started:
        //            LastScale = e.Scale;
        //            StartScale = Scale;
        //            AnchorX = e.ScaleOrigin.X;
        //            AnchorY = e.ScaleOrigin.Y;
        //            break;
        //        case GestureStatus.Running:
        //            // new scale is < 0 or the difference of last and new scale is > 30% of the last scale
        //            if (e.Scale < 0 || Math.Abs(LastScale - e.Scale) > LastScale * 0.3)
        //            { return; }
        //            LastScale = e.Scale;
        //            // calculate the new scale to change the canvas to
        //            Scale = Clamp(Scale + (e.Scale - 1) * StartScale, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
        //            Debug.Print($"--- Scale Arguments ---");
        //            Debug.Print($"Scale: {Scale}");
        //            Debug.Print($"StartScale: {StartScale}");
        //            Debug.Print($"LastScale: {LastScale}");
        //            Debug.Print($"ScaleOrigin: {e.ScaleOrigin}");
        //            Debug.Print($"--- Scale Arguments ---");
        //            break;
        //        case GestureStatus.Completed:
        //            // check if the new scale is within the min and max scale
        //            if (Scale < MIN_SCALE)
        //            {
        //                this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
        //            }
        //            else if (Scale > MAX_SCALE)
        //            {
        //                this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
        //            }
        //            break;

        //    }
        //}

        private void ZoomAndTranslate(SKCanvas canvas)
        {
            // Update the previous zoom and pan offset
            canvas.Translate(PanOffset.X, PanOffset.Y);
            canvas.Scale(Zoom);
        }

    }
}
