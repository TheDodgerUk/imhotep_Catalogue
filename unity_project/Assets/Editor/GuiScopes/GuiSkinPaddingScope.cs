using UnityEngine;

public class GuiSkinPaddingScope : GUI.Scope
{
    private readonly GUIStyle m_Style;
    private readonly RectOffset m_Padding;
    private readonly RectOffset m_Overflow;
    private readonly RectOffset m_Margin;

    public GuiSkinPaddingScope(GUIStyle style, int padding)
    {
        m_Overflow = Clone(style.overflow);
        m_Padding = Clone(style.padding);
        m_Margin = Clone(style.margin);

        style.overflow = Create(padding);
        style.padding = Create(padding);
        style.margin = Create(padding);

        m_Style = style;
    }

    protected override void CloseScope()
    {
        m_Style.overflow = m_Overflow;
        m_Style.padding = m_Padding;
        m_Style.margin = m_Margin;
    }

    private static RectOffset Clone(RectOffset r)
    {
        return new RectOffset(r.left, r.right, r.top, r.bottom);
    }

    private static RectOffset Create(int p) => new RectOffset(p, p, p, p);
}