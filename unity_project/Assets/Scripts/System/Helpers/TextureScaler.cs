// Only works on ARGB32, RGB24 and Alpha8 textures that are marked readable

using System.Threading;
using UnityEngine;

public class TextureScaler
{
    public class ThreadData
    {
        public int start;
        public int end;
        public ThreadData(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    private static Color[] m_OldColors;
    private static Color[] m_NewColors;

    private static float m_RatioX;
    private static float m_RatioY;
    private static int m_W2;
    private static int m_W;

    private static int m_FinishCount;
    private static Mutex m_Mutex;

    public static void To(Texture2D tex, int w, int h, bool pointFiltering = false)
    {
        m_W = tex.width;
        m_W2 = w;

        m_OldColors = tex.GetPixels();
        m_NewColors = new Color[m_W2 * h];

        if (pointFiltering == true)
        {
            m_RatioX = m_W / (float)m_W2;
            m_RatioY = tex.height / (float)h;
        }
        else
        {
            m_RatioX = 1f / (m_W2 / (float)(m_W - 1));
            m_RatioY = 1f / (h / (float)(tex.height - 1));
        }

        var cores = Mathf.Min(SystemInfo.processorCount, h);
        var slice = h / cores;

        m_FinishCount = 0;
        if (m_Mutex == null)
        {
            m_Mutex = new Mutex(false);
        }

        if (cores > 1)
        {
            int i = 0;
            ThreadData threadData = null;
            for (i = 0; i < cores - 1; i++)
            {
                threadData = new ThreadData(slice * i, slice * (i + 1));

                var thread = new Thread(GetThreadStartDelegate(pointFiltering));
                thread.Start(threadData);
            }

            threadData = new ThreadData(slice * i, h);
            if (pointFiltering == true)
            {
                PointScale(threadData);
            }
            else
            {
                BilinearScale(threadData);
            }
            while (m_FinishCount < cores)
            {
                Thread.Sleep(1);
            }
        }
        else
        {
            var threadData = new ThreadData(0, h);
            if (pointFiltering == true)
            {
                PointScale(threadData);
            }
            else
            {
                BilinearScale(threadData);
            }
        }

        tex.Reinitialize(m_W2, h);
        tex.SetPixels(m_NewColors);
        tex.Apply();

        m_OldColors = null;
        m_NewColors = null;
    }

    private static ParameterizedThreadStart GetThreadStartDelegate(bool point)
    {
        if (point == true)
        {
            return new ParameterizedThreadStart(PointScale);
        }
        else
        {
            return new ParameterizedThreadStart(BilinearScale);
        }
    }

    private static void BilinearScale(object data)
    {
        var threadData = (ThreadData)data;
        for (int y = threadData.start; y < threadData.end; y++)
        {
            int yFloor = (int)(y * m_RatioY);
            int y1 = (yFloor + 0) * m_W;
            int y2 = (yFloor + 1) * m_W;
            int yW = y * m_W2;

            for (int x = 0; x < m_W2; x++)
            {
                int xFloor = (int)(x * m_RatioX);
                float xLerp = x * m_RatioX - xFloor;
                m_NewColors[yW + x] = Lerp(Lerp(m_OldColors[y1 + xFloor], m_OldColors[y1 + xFloor + 1], xLerp),
                                         Lerp(m_OldColors[y2 + xFloor], m_OldColors[y2 + xFloor + 1], xLerp),
                                         y * m_RatioY - yFloor);
            }
        }

        m_Mutex.WaitOne();
        m_FinishCount++;
        m_Mutex.ReleaseMutex();
    }

    private static void PointScale(object data)
    {
        var threadData = (ThreadData)data;
        for (int y = threadData.start; y < threadData.end; y++)
        {
            int thisY = (int)(m_RatioY * y) * m_W;
            int yw = y * m_W2;
            for (var x = 0; x < m_W2; x++)
            {
                m_NewColors[yw + x] = m_OldColors[(int)(thisY + m_RatioX * x)];
            }
        }

        m_Mutex.WaitOne();
        m_FinishCount++;
        m_Mutex.ReleaseMutex();
    }

    private static Color Lerp(Color c1, Color c2, float t)
    {
        c2.r = c1.r + (c2.r - c1.r) * t;
        c2.g = c1.g + (c2.g - c1.g) * t;
        c2.b = c1.b + (c2.b - c1.b) * t;
        c2.a = c1.a + (c2.a - c1.a) * t;
        return c2;
    }
}