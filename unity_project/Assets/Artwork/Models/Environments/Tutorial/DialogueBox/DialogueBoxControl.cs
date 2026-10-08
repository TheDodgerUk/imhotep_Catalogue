using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class DialogueBoxControl : MonoBehaviour
{
    public string m_Text;
    private TextMeshProUGUI m_TextBox;

    private void Awake()
    {
        //Get Text box reference
        m_TextBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string sText)
    {
        m_Text = sText;

        if(null != m_TextBox)
        {
            m_TextBox.text = m_Text;
        }
    }
}