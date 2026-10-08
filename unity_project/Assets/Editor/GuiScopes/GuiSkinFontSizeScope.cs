using UnityEngine;

public class GuiSkinFontSizeScope : GUI.Scope
{
    private readonly GUIStyle m_Style;
    private readonly int m_FontSize;

    public GuiSkinFontSizeScope(GUIStyle style, int fontSize)
    {
        m_FontSize = style.fontSize;
        style.fontSize = fontSize;
        m_Style = style;
    }

    protected override void CloseScope()
    {
        m_Style.fontSize = m_FontSize;
    }
}