using System;

public class TaskAction
{
    private Action m_Callback;
    private int m_Current;
    private int m_Count;

    public TaskAction(int count, Action callback)
    {
        if (count == 0)
        {
            callback?.Invoke();
        }
        else
        {
            m_Callback = callback;
            m_Count = count;
        }
    }

    public void Increment()
    {
        m_Current++;

        if (m_Current == m_Count)
        {
            m_Callback?.Invoke();
        }
    }
}
