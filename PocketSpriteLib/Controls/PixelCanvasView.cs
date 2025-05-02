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

        private SKPoint CenterOffset {
            get {
                float canvasWidth = this.CanvasSize.Width;
                float canvasHeight = this.CanvasSize.Height;

                float contentWidth = _pixelCanvas.Width * pixelSize * Zoom;
                float contentHeight = _pixelCanvas.Height * pixelSize * Zoom;

                return new SKPoint(
                    (canvasWidth - contentWidth) / 2f,
                    (canvasHeight - contentHeight) / 2f
                );
            }
        }

        private PixelCanvas _pixelCanvas;
        private int pixelSize = 8; // Default pixel size
        private SKPoint _lastTouchPoint;
        private bool _isDrawing = false; // Flag to indicate if drawing is in progress

        public int PixelSize {
            get => pixelSize;
            set {
                pixelSize = value;
                InvalidateSurface(); // Redraw the canvas when pixel size changes
            }
        }
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

        private SKPoint ScreenToGrid(SKPoint screen) {
            var canvasPos = new SKPoint(
                (screen.X - PanOffset.X - CenterOffset.X) / Zoom,
                (screen.Y - PanOffset.Y - CenterOffset.Y) / Zoom
            );

            return new SKPoint(
                canvasPos.X / pixelSize,
                canvasPos.Y / pixelSize
            );
        }

        private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            // Get the pixel size from the canvas

            int canvasPixelWidth = _pixelCanvas.Width;
            int canvasPixelHeight = _pixelCanvas.Height;
            int contentWidth = canvasPixelWidth * pixelSize;
            int contentHeight = canvasPixelHeight * pixelSize;
            float offsetX = (e.Info.Width - contentWidth * Zoom) / 2f;
            float offsetY = (e.Info.Height - contentHeight * Zoom) / 2f;

            var canvas = e.Surface.Canvas;
            canvas.Clear(new SKColor(30, 30, 30)); // dark grey background

            canvas.Save();

            // Apply pan and zoom
            canvas.Translate(offsetX + PanOffset.X, offsetY + PanOffset.Y);
            canvas.Scale(Zoom);

            // Draw checkerboard background behind the pixel canvas
            DrawingUtils.DrawCheckerboard(canvas, contentWidth, contentHeight,
                pixelSize * (Math.Min(canvasPixelHeight/pixelSize, canvasPixelWidth/pixelSize) / Math.Clamp(((int)Zoom), 1, 1000)));

            // Draw pixel layers
            DrawingUtils.Draw(canvas, canvasPixelWidth, canvasPixelHeight, contentWidth, contentHeight, pixelSize, _pixelCanvas.LayerManager.Layers);
            
            // Draw the Grid
            DrawingUtils.DrawGridOverlay(canvas, offsetX, offsetY, canvasPixelWidth, canvasPixelHeight, Zoom, pixelSize);

            canvas.Restore();
            // restore the canvas state
        }
        private void OnTouch(object? sender, SKTouchEventArgs e)
        {

            var adjustedPoint = ScreenToGrid(e.Location);
            // boundary check
            if (adjustedPoint.X < 0 || adjustedPoint.X >= _pixelCanvas.Width || adjustedPoint.Y < 0 || adjustedPoint.Y >= _pixelCanvas.Height) {
                e.Handled = false; // Ignore touch events outside the canvas bounds
                return;
            }

            // handle touch events
            if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
            {
                _isDrawing = false;
                _lastTouchPoint = SKPoint.Empty;
                e.Handled = false;
                return;
            }

            if (e.ActionType == SKTouchAction.Pressed)
            {
                // Start drawing
                _isDrawing = true;
                _lastTouchPoint = new SKPoint(adjustedPoint.X, adjustedPoint.Y);
                _pixelCanvas.SetPixel((int)adjustedPoint.X, (int)adjustedPoint.Y, SKColors.Black);
                e.Handled = true;
            }
            else if (_isDrawing && e.ActionType == SKTouchAction.Moved)
            {
                int lastGridx = (int)_lastTouchPoint.X;
                int lastGridy = (int)_lastTouchPoint.Y;

                DrawingUtils.DrawPixelLineOnLayer(_pixelCanvas, lastGridx, (int)adjustedPoint.X, lastGridy, (int)adjustedPoint.Y, SKColors.Black);
                _lastTouchPoint = adjustedPoint;
                e.Handled = true;
            }

            InvalidateSurface();
          
        }
        private void OnMouseWheel(object? sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.WheelChanged)
            {
                float zoomDelta = e.WheelDelta > 0 ? 1.1f : 0.9f;
                float newZoom = Zoom * zoomDelta;
                float clampedZoom = Math.Clamp(newZoom, MIN_ZOOM, MAX_ZOOM);

                PrevZoom = Zoom;

                // 1. Convert screen mouse position to canvas-local space
                var localMouse = new SKPoint(
                    (e.Location.X - CenterOffset.X - PanOffset.X) / Zoom,
                    (e.Location.Y - CenterOffset.Y - PanOffset.Y) / Zoom
                );

                // 2. Apply zoom
                Zoom = newZoom;

                // 3. Calculate new screen position of that same point
                var newScreenPos = new SKPoint(
                    localMouse.X * Zoom + CenterOffset.X + PanOffset.X,
                    localMouse.Y * Zoom + CenterOffset.Y + PanOffset.Y
                );

                // 4. Adjust pan so the point stays under cursor
                PanOffset.X -= newScreenPos.X - e.Location.X;
                PanOffset.Y -= newScreenPos.Y - e.Location.Y;

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

    }
}
