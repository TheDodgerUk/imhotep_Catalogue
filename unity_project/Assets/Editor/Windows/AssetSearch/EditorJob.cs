using System;
using UnityEditor;
using UnityEngine;

public class EditorJob
{
    public GUIContent Title { get; private set; }
    public float WaitBetweenJobs { get; set; }

    private Action m_OnComplete;
    private Action<int> m_OnWork;

    private float m_WaitUntil;
    private int m_Current;
    private int m_Max;

    public EditorJob(string title, Action<int> onWork, Action onComplete, int max)
    {
        Title = new GUIContent(title);
        m_OnComplete = onComplete;
        m_OnWork = onWork;

        m_Current = 0;
        m_Max = max;
    }

    public bool ReadyToContinue()
    {
        return (EditorApplication.timeSinceStartup >= m_WaitUntil);
    }

    public void DoWork()
    {
        if (m_Current >= m_Max)
        {
            m_OnComplete();
        }
        else
        {
            m_WaitUntil = (float)EditorApplication.timeSinceStartup + WaitBetweenJobs;

            m_OnWork(m_Current);
            m_Current++;
        }
    }

    public void Cancel()
    {
        m_OnComplete();
    }
}
