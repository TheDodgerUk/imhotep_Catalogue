using UnityEngine;

public class GuiSkinTextColourScope : GUI.Scope
{
    private readonly GUIStyleState m_StyleState;
    private readonly Color m_Colour;

    public GuiSkinTextColourScope(GUIStyleState styleState, Color colour)
    {
        m_Colour = styleState.textColor;
        styleState.textColor = colour;
        m_StyleState = styleState;
    }

    protected override void CloseScope()
    {
        m_StyleState.textColor = m_Colour;
    }
}