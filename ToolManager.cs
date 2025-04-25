using System;
using System.Collections;
using Microsoft.Maui.Controls.Platform;

namespace PocketSprite;

public class ToolManager
{
    public List<Tool> _toolSet = new List<Tool>();
    private Tool activeTool;
    private Tool brushTool;
    public ToolManager(Tool active) {
        activeTool = active;
    }

    // Parse tools in a json and create the tools, adding them to the toolset
    // as they are created
    private void InitializeTools() {
        
    }

    public void SetActiveTool(Tool tool) {
        GetActiveTool().isActive = false;
        tool.isActive = true;
    }

    public Tool GetActiveTool() {
        foreach (Tool tool in _toolSet) {
            if (tool.isActive)
                return tool;
        }
        return brushTool;
    }
}
