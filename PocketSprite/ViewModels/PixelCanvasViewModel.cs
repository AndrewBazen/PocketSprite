using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;
using SkiaSharp;
using PocketSprite.Models;
using PocketSpriteLib;
using System.Threading.Tasks;

namespace PocketSprite.ViewModels
{
    internal class PixelCanvasViewModel : ObservableObject
    {

        private PixelCanvas _pixelCanvas;
        private int _scaleFactor;
        public int PixelCanvasWidth => _pixelCanvas.Width;
        public int PixelCanvasHeight => _pixelCanvas.Height;

        public int ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (SetProperty(ref _scaleFactor, value))
                {
                    // Update the canvas size based on the scale factor
                    _pixelCanvas.Width = PixelCanvasWidth * value;
                    _pixelCanvas.Height = PixelCanvasHeight * value;
                }
            }
        }

        public int PixelSize => _pixelCanvas.PixelSize;

       
        public PixelCanvasViewModel()
        {
            // Initialize the pixel canvas with default values

            _pixelCanvas = new PixelCanvas();
        }

        public async void OnTouch(object sender, SKTouchEventArgs e)
        {
            await _pixelCanvas.HandleTouch(sender, e);
           
        }

        public async Task PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            // Clear the canvas
            canvas.Clear(SKColors.White);
            // let the view model handle the drawing

            // Draw the current layer
            await _pixelCanvas.InitializeCanvas();
        }
    }
}
