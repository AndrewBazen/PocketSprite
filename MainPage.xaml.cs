using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;

namespace PocketSprite;

public partial class MainPage : ContentPage
{
    private LayerManager _layerManager;

	private ToolManager _toolManager;
	private SKPoint? _lastTouchPoint; // Store the last touch point
	private int _pixelSize = 6; // Size of each grid pixel in canvas units

    public MainPage()
    {
        InitializeComponent();

        int canvasWidth = 256;   // Grid width in pixels
        int canvasHeight = 256;  // Grid height in pixels

        // Initialize the layer manager
        _layerManager = new LayerManager(canvasWidth / _pixelSize, canvasHeight / _pixelSize, _pixelSize);

		var currentLayer = _layerManager.currentLayer;

        // Set some example pixels
        currentLayer.SetPixel(10, 10, SKColors.Red);
        currentLayer.SetPixel(20, 20, SKColors.Blue);
        currentLayer.SetPixel(30, 30, SKColors.Green);

        // Redraw the canvas
        PixelCanvasView.InvalidateSurface();
    }

    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;

        // Clear the canvas
        canvas.Clear(SKColors.White);

        // Draw the layers
        _layerManager.Draw(canvas);

		    // Draw grid overlay
    	DrawGridOverlay(canvas, e.Info.Width, e.Info.Height, _pixelSize);
    }

	private bool doPaint = false;
	private void OnCanvasViewTouch(object sender, SKTouchEventArgs e)
	{
		if (e.ActionType == SKTouchAction.Pressed) { doPaint = true; }
        if (e.ActionType == SKTouchAction.Released) { doPaint = false; }
        if (doPaint && e.ActionType == SKTouchAction.Moved)
		{
			var canvasView = (SKCanvasView)sender;

			// Get the canvas scale (use the same logic as in Draw)
			float canvasWidth = canvasView.CanvasSize.Width;
			float canvasHeight = canvasView.CanvasSize.Height;

			float gridWidth = _layerManager._width * _pixelSize;
			float gridHeight = _layerManager._height * _pixelSize;

			// Calculate the scale factor applied during rendering
			float scaleX = canvasWidth / gridWidth;
			float scaleY = canvasHeight / gridHeight;
			float scale = Math.Min(scaleX, scaleY);

			// Adjust touch coordinates to match the scaled grid
			float adjustedX = e.Location.X / scale;
			float adjustedY = e.Location.Y / scale;

			// Convert adjusted coordinates to logical grid coordinates
			int currentX = (int)(adjustedX / _pixelSize);
			int currentY = (int)(adjustedY / _pixelSize);

			// Ensure the coordinates are within bounds
			if (currentX >= 0 && currentX < _layerManager._width && currentY >= 0 && currentY < _layerManager._height)
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
			} else {
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

	private void DrawLineBetweenPoints(int x0, int y0, int x1, int y1, SKColor color)
	{
		int dx = Math.Abs(x1 - x0);
		int dy = Math.Abs(y1 - y0);
		int sx = x0 < x1 ? 1 : -1;
		int sy = y0 < y1 ? 1 : -1;
		int err = dx - dy;

		while (true)
		{
			// Set the pixel at the current point
			_layerManager.currentLayer.SetPixel(x0, y0, color);

			// Break if we've reached the end point
			if (x0 == x1 && y0 == y1) break;

			int e2 = err * 2;

			if (e2 > -dy)
			{
				err -= dy;
				x0 += sx;
			}

			if (e2 < dx)
			{
				err += dx;
				y0 += sy;
			}
		}
	}

	private void DrawGridOverlay(SKCanvas canvas, int canvasWidth, int canvasHeight, int pixelSize)
	{
		float gridWidth = _layerManager._width * _pixelSize;
		float gridHeight = _layerManager._height * _pixelSize;

		// Calculate the scale factor applied during rendering
		float scaleX = canvasWidth / gridWidth;
		float scaleY = canvasHeight / gridHeight;
		float scale = Math.Min(scaleX, scaleY);

		// Draw the grid overlay using the scaled pixel size
		int scaledPixelSize = (int)(_pixelSize * scale);
	
		using var gridPaint = new SKPaint
		{
			Color = SKColors.Gray.WithAlpha(128), // Semi-transparent gray
			StrokeWidth = 0.5f,                     // Thin lines
			Style = SKPaintStyle.Stroke
		};

		// Draw vertical grid lines
		for (int x = 0; x <= gridWidth; x += pixelSize)
		{
			canvas.DrawLine(x, 0, x, gridHeight, gridPaint);
		}

		// Draw horizontal grid lines
		for (int y = 0; y <= gridHeight; y += pixelSize)
		{
			canvas.DrawLine(0, y, gridWidth, y, gridPaint);
		}
	}
}

