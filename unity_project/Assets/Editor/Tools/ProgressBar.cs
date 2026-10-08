using System;
using System.Collections;
using UnityEditor;

public class ProgressBar : IDisposable
{
    private int m_UpdateIndex = 0;
    private int m_MaxIncrement;
    private int m_Increment;
    private float m_Progress;

    private string m_Subtitle;
    private string m_Title;
    private string m_Info;

    public ProgressBar(string title) : this(title, string.Empty) { }
    public ProgressBar(string title, string info) : this(title, info, 0) { }
    public ProgressBar(string title, int maxIncrement) : this(title, string.Empty, maxIncrement) { }
    public ProgressBar(string title, ICollection list) : this(title, string.Empty, list.Count) { }
    public ProgressBar(string title, string info, int maxIncrement)
    {
        SetMaxIncrement(maxIncrement);
        m_Subtitle = string.Empty;
        m_Title = title;
        m_Info = info;

        Display();
    }

    public void SetMaxIncrement(ICollection list) => SetMaxIncrement(list.Count);
    public void SetMaxIncrement(int maxIncrement)
    {
        m_UpdateIndex = (int)(maxIncrement / 100f).Max(1);
        m_MaxIncrement = maxIncrement;
    }

    public void SetSubtitle(string subtitle)
    {
        m_Subtitle = subtitle;
    }

    public void Dispose()
    {
        Clear();
    }

    public bool UpdateIncrement() => UpdateIncrement(m_Info);
    public bool UpdateIncrement(string info) => UpdateIncrement(info, m_Increment + 1);
    public bool UpdateIncrement(string info, int increment)
    {
        m_Increment = increment;
        m_Progress = m_Increment / (float)m_MaxIncrement;
        return UpdateInfo(info);
    }

    public bool UpdateInfo(string info)
    {
        m_Info = info;
        return Display();
    }

    private bool Display()
    {
        if ((m_Increment % m_UpdateIndex) == 0)
        {
            string title = string.Concat(m_Title, m_Subtitle);
            return Show(title, m_Info, m_Progress);
        }
        return false;
    }

    public static bool Show(string title, string info, float progress)
    {
        title = $"{title} ({(progress * 100):N0}%)";
        bool cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progress);
        if (cancel == true)
        {
            Clear();
        }
        return cancel;
    }

    public static void Clear()
    {
        EditorUtility.ClearProgressBar();
    }
}
