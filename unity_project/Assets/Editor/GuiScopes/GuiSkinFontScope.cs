using UnityEngine;

public class GuiSkinFontScope : GUI.Scope
{
    private readonly GUIStyle m_Style;
    private readonly Font m_Font;

    public GuiSkinFontScope(GUIStyle style, Font font)
    {
        m_Font = style.font;
        style.font = font;
        m_Style = style;
    }

    protected override void CloseScope()
    {
        m_Style.font = m_Font;
    }
}