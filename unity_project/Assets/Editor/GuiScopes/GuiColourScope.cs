using UnityEngine;

public class GuiColourScope : GUI.Scope
{
    private readonly Color m_BackgroundColour;
    private readonly Color m_ForegroundColour;
    private readonly Color m_ContentColour;

    public GuiColourScope(Color colour) : this(fg: colour) { }

    public GuiColourScope(Color? fg = null, Color? bg = null, Color? content = null)
    {
        m_BackgroundColour = GUI.backgroundColor;
        m_ContentColour = GUI.contentColor;
        m_ForegroundColour = GUI.color;

        GUI.backgroundColor = bg ?? m_BackgroundColour;
        GUI.contentColor = content ?? m_ContentColour;
        GUI.color = fg ?? m_ForegroundColour;
    }

    protected override void CloseScope()
    {
        GUI.backgroundColor = m_BackgroundColour;
        GUI.contentColor = m_ContentColour;
        GUI.color = m_ForegroundColour;
    }

}