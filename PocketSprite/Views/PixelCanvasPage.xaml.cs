/* PixelCanvasPage.xaml.cs
 *
 * @Description: This file contains the logic for the PixelCanvasPage class, which is the main page for the PixelCanvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 * @version: 1.0
 * @license: MIT License
 */
using System.Runtime.Versioning;

namespace PocketSprite.Views;

/* @Class: PixelCanvasPage
 *
 * @Description: This class is the main page for the PixelCanvas.
 * @Author: Andrew Bazen
 * @Date: 2025-4-24
 */
[SupportedOSPlatform("windows")]
[SupportedOSPlatform("android")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("maccatalyst")]
public partial class PixelCanvasPage : ContentPage
{

    /* @Constructor: PixelCanvasPage
     *
     * @Description: Constructor for the PixelCanvasPage class.
     */
    public PixelCanvasPage()
    {
        InitializeComponent();
    }

}