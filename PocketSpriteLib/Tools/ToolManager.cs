using System.Collections.Generic;



namespace PocketSpriteLib.Tools;

public class Tool
{
    public bool isActive;
    public Tool(bool _isActive) {
        isActive = _isActive;
    }


}

public class ToolManager
{
    public List<Tool> _toolSet = new List<Tool>();
    private Tool activeTool;
    public ToolManager(Tool active)
    {
        activeTool = active;
        _toolSet.Add(activeTool);
    }

    public void SetActiveTool(Tool tool)
    {
        GetActiveTool().isActive = false;
        tool.isActive = true;
    }

    public Tool GetActiveTool()
    {
        foreach (Tool tool in _toolSet)
        {
            if (tool.isActive)
                return tool;
        }
        return new Tool(true);
    }
}
