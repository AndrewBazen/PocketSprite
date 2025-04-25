using SkiaSharp;


namespace PocketSprite
{
    internal class DrawToCanvas
    {
        public static void Draw(SKCanvas? canvas, int width, int height, int pixelSize, List<CanvasLayer> layers)
        {
            // get the canvas size
            float canvasWidth = canvas.DeviceClipBounds.Width;
            float canvasHeight = canvas.DeviceClipBounds.Height;

            // Calculate rendered grid size
            float gridWidth = width * pixelSize;
            float gridHeight = height * pixelSize;

            // Calculate scale factor
            float scaleX = canvasWidth / gridWidth;
            float scaleY = canvasHeight / gridHeight;

            // Use the smaller scale factor to fit the grid within the canvas
            float scale = Math.Min(scaleX, scaleY);

            canvas.Scale(scale);
            foreach (CanvasLayer layer in layers)
            {
                if (layer.IsVisible)
                {
                    float renderedWidth = layer.Bitmap.Width * pixelSize;
                    float renderedHeight = layer.Bitmap.Height * pixelSize;
                    Console.WriteLine($"Rendered Grid Size: {renderedWidth}x{renderedHeight}");
                    using var paint = new SKPaint
                    {
                        Color = SKColors.White.WithAlpha((byte)(layer.Opacity * 255))
                    };

                    // Scale the bitmap
                    var destRect = new SKRect(0, 0, layer.Bitmap.Width * pixelSize, layer.Bitmap.Height * pixelSize);
                    canvas.DrawBitmap(layer.Bitmap, destRect, paint);

                    // Debug rectangle
                    using var debugPaint = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
                    canvas.DrawRect(destRect, debugPaint); // Draw a red border around the grid
                }
            }
        }
    }
}
