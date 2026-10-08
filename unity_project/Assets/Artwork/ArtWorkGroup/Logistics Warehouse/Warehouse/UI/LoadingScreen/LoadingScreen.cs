using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private Image m_LoadingRotator;
    private CanvasGroup m_CanvasGroup;

    [SerializeField]
    private float m_AnglePerSecond = 22.5f;
    private Quaternion rotationIncrement;

    private float m_fTimer = 0f;
    [SerializeField]
    private float m_fTimerIncrement = 0.1f;

    // Start is called before the first frame update
    public void Init()
    {
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
        m_LoadingRotator = transform.SearchComponent<Image>("LoadingRotator");

        rotationIncrement = Quaternion.Euler(0f, 0f, m_AnglePerSecond);
        Show(false);
    }

    public void Show(bool bShow)
    {
        if (true == bShow)
        {
            enabled = true;
            m_CanvasGroup.alpha = 1f;
            m_CanvasGroup.blocksRaycasts = true;
        }
        else
        {
            enabled = false;
            m_CanvasGroup.alpha = 0f;
            m_CanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update()
    {
        if (null != m_LoadingRotator)
        {
            m_fTimer -= Time.deltaTime;
            if (m_fTimer < 0f)
            {
                m_LoadingRotator.rectTransform.rotation *= rotationIncrement;
                m_fTimer = m_fTimerIncrement;
            }
        }
    }
}
