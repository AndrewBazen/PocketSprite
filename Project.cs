using System;

namespace PocketSprite;

public class Project
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int PixelSize { get; set; }
    
    public Project(string name, int width, int height, int pixelSize)
    {
        Name = name;
        Width = width;
        Height = height;
        PixelSize = pixelSize;
    }


}
