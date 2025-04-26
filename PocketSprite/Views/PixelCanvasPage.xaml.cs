using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp;
using PocketSprite.ViewModels;
using System.Threading.Tasks;

namespace PocketSprite.Views;

public partial class PixelCanvasPage : ContentPage
{
    public PixelCanvasPage()
    {
        InitializeComponent();
    }

    public async Task OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        // Clear the canvas
        canvas.Clear(SKColors.White);

        // let the view model handle the drawing
        if (BindingContext is PixelCanvasViewModel viewModel)
        {
            //invoke the paint surface method in the view model
            await viewModel.PaintSurface(sender, e);
        }

    }

    public void OnCanvasViewTouch(object sender, SKTouchEventArgs e)
    {
        if (BindingContext is PixelCanvasViewModel viewModel)
        {
            //invoke the touch event method in the view model
            viewModel.OnTouch(sender, e);
        }
       
    }
    
// private void OnCanvasViewTouch(object sender, SKTouchEventArgs e)
// 	{
// 		if (e.ActionType == SKTouchAction.Pressed) { doPaint = true; }
// 		if (e.ActionType == SKTouchAction.Released) { doPaint = false; }
// 		if (doPaint && e.ActionType == SKTouchAction.Moved)
// 		{
// 			var canvasView = (SKCanvasView)sender;

// 			// Get the canvas scale (use the same logic as in Draw)
// 			float canvasViewWidth = canvasView.CanvasSize.Width;
// 			float canvasViewHeight = canvasView.CanvasSize.Height;

// 			// float gridWidth = _layerManager.Width * _pixelSize;
// 			// float gridHeight = _layerManager.Height * _pixelSize;

// 			// Calculate the scale factor applied during rendering
// 			float scaleX = canvasViewWidth / gridWidth;
// 			float scaleY = canvasViewHeight / gridHeight;
// 			float scale = Math.Min(scaleX, scaleY);

// 			// Adjust touch coordinates to match the scaled grid
// 			float adjustedX = e.Location.X / scale;
// 			float adjustedY = e.Location.Y / scale;

// 			// Convert adjusted coordinates to logical grid coordinates
// 			int currentX = (int)(adjustedX / _pixelSize);
// 			int currentY = (int)(adjustedY / _pixelSize);

// 			// Ensure the coordinates are within bounds
// 			if (currentX >= 0 && currentX < _layerManager.Width && currentY >= 0 && currentY < _layerManager.Height)
// 			{
// 				// Draw a line from the last point to the current point
// 				if (_lastTouchPoint != null)
// 				{
// 					int lastX = (int)(_lastTouchPoint.Value.X / _pixelSize);
// 					int lastY = (int)(_lastTouchPoint.Value.Y / _pixelSize);

// 					DrawLineBetweenPoints(lastX, lastY, currentX, currentY, SKColors.Black);
// 				}

// 				// Update the last touch point
// 				_lastTouchPoint = new SKPoint(adjustedX, adjustedY);

// 				// Redraw the canvas
// 				PixelCanvasView.InvalidateSurface();
// 			}
// 			else
// 			{
// 				_lastTouchPoint = null;
// 			}

// 			e.Handled = true;
// 		}
// 		// touch is released or moves outside the canvas
// 		else if (e.ActionType == SKTouchAction.Released || e.ActionType == SKTouchAction.Exited)
// 		{
// 			// Clear the last touch point when the touch is released or moves outside the canvas
// 			_lastTouchPoint = null;

// 			e.Handled = true;
// 		}
// 	}
}