using SkiaSharp.Views.Maui;
using SkiaSharp;
using PocketSprite.ViewModels;

namespace PocketSprite.Views;

public partial class PixelCanvasPage : ContentPage
{
	public PixelCanvasPage()
	{
		InitializeComponent();
	}

	public void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        // Clear the canvas
        canvas.Clear(SKColors.White);
        // Draw the layers
        _layerManager.DrawLayers(canvas);
        // Draw grid overlay
        DrawGridOverlay(canvas, e.Info.Width, e.Info.Height, _pixelSize);
    }

    public void OnCanvasViewTouch(object sender, SKTouchEventArgs e)
    {
        if (BindingContext is PixelCanvasViewModel viewModel)
        {
            viewModel.CanvasTouchCommand?.Execute(e);
        }
        e.Handled = true;

        if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
        {
            var canvasView = (SKCanvasView)sender;

            

            // Ensure the coordinates are within bounds
            if (currentX >= 0 && currentX < _layerManager.Width && currentY >= 0 && currentY < _layerManager.Height)
            {
                // Draw a line from the last point to the current point
                if (_lastTouchPoint != null)
                {
                    int lastX = (int)(_lastTouchPoint.Value.X / _pixelSize);
                    int lastY = (int)(_lastTouchPoint.Value.Y / _pixelSize);

                    DrawLineBetweenPoints(lastX, lastY, currentX, currentY, SKColors.Black);
                }

                // Update the last touch point
                _lastTouchPoint = new SKPoint(adjustedX, adjustedY);

                // Redraw the canvas
                PixelCanvasView.InvalidateSurface();
            }
            else
            {
                _lastTouchPoint = null;
            }

            e.Handled = true;
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