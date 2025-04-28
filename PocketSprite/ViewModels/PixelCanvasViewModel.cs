/* PixelCanvasViewModel.cs
 *
 * @Description: This file contains the logic for the PixelCanvasViewModel class, which is the view model for the PixelCanvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @license: MIT License
 */
using CommunityToolkit.Mvvm.ComponentModel;
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

        /* @Constructor: PixelCanvasViewModel
         *
         * @Description: Constructor for the PixelCanvasViewModel class.
         */
        public PixelCanvasViewModel()
        {

        }
    }
}
