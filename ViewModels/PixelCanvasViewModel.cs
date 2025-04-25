using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;
using SkiaSharp;
using PocketSprite.Models;
using System.Windows.Input;

namespace PocketSprite.ViewModels
{
    internal class PixelCanvasViewModel : ObservableObject
    {


        private PixelCanvas _pixelCanvas;
        public LayerManager LayerManager { get; private set; }
        public int PixelSize
        {
            get => _pixelCanvas.PixelSize;
            set
            {
                if (_pixelCanvas.PixelSize != value)
                {
                    _pixelCanvas.PixelSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CanvasWidth 
        {
            get => _pixelCanvas.Width;
            set
            {
                if (_pixelCanvas.Width != value)
                {
                    _pixelCanvas.Width = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CanvasHeight
        {
            get => _pixelCanvas.Height;
            set
            {
                if (_pixelCanvas.Height != value)
                {
                    _pixelCanvas.Height = value;
                    OnPropertyChanged();
                }
            }
        }

        public LayerManager LayerManger => _pixelCanvas.LayerManager;

        public SKCanvas MainCanvas => _pixelCanvas.MainPixelCanvas;

        public SKBitmap MainBitmap => _pixelCanvas.MainPixelBitmap;

        public ICommand CanvasTouchCommand { get; private set; }

        public PixelCanvasViewModel()
        {
            _pixelCanvas = new PixelCanvas();

            CanvasTouchCommand = new Command<SKTouchEventArgs>(Touch);
        }

        private async Task Touch(object sender, SKTouchEventArgs e)
        {
            if (e.ActionType == SKTouchAction.Pressed || e.ActionType == SKTouchAction.Moved)
            {
                var canvasView = (SKCanvasView)sender;
                // Get the canvas scale (use the same logic as in Draw)
                SKPoint adjustedTouchPoint = _pixelCanvas.ConvertCoordinates(e.Location, canvasView);
                // Handle touch event
               await LayerManager.HandleTouch(adjustedTouchPoint, e.ActionType);
            }
        }

        public void PaintSurface()
        {
            var canvas = e.Surface.Canvas;
            // Clear the canvas
            canvas.Clear(SKColors.White);
            // Draw the layers
            LayerManager.DrawLayers(canvas);
            // Draw grid overlay
            _pixelCanvas.DrawGridOverlay(canvas, e.Info.Width, e.Info.Height);
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(PixelSize));
            OnPropertyChanged(nameof(CanvasWidth));
            OnPropertyChanged(nameof(CanvasHeight));
        }
    }
}
