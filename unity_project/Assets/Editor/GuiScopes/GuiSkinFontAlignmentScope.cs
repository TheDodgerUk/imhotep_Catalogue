using UnityEngine;

public class GuiSkinFontAlignmentScope : GUI.Scope
{
    private readonly GUIStyle m_Style;
    private readonly TextAnchor m_Alignment;

    public GuiSkinFontAlignmentScope(GUIStyle style, TextAnchor alignment)
    {
        m_Alignment = style.alignment;
        style.alignment = alignment;
        m_Style = style;
    }

    protected override void CloseScope()
    {
        m_Style.alignment = m_Alignment;
    }
}