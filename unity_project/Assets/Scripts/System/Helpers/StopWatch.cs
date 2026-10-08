using System;
using UnityEngine;

public abstract class Stopwatch
{
    private enum WatchType
    {
        UnityTime,
        RealTime,
    }

    public float TotalMilliseconds { get; private set; }
    public float TotalSeconds { get; private set; }

    public string Readable()
    {
        TimeSpan t = TimeSpan.FromMilliseconds(TotalMilliseconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
               (int)t.TotalHours,
               t.Minutes,
               t.Seconds);
    }

    public abstract void Reset();

    public abstract void Stop();


    public static Stopwatch Diagnostics() => new DiagnosticStopwatch();
    public static Stopwatch UnityTime() => new UnityStopwatch(WatchType.UnityTime);
    public static Stopwatch RealTime() => new UnityStopwatch(WatchType.RealTime);

    private class DiagnosticStopwatch : Stopwatch
    {
        private System.Diagnostics.Stopwatch m_Stopwatch;

        public DiagnosticStopwatch()
        {
            m_Stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }

        public override void Reset()
        {
            m_Stopwatch.Reset();
            m_Stopwatch.Start();
        }

        public override void Stop()
        {
            m_Stopwatch.Stop();
            TotalMilliseconds = m_Stopwatch.ElapsedMilliseconds;
            TotalSeconds = TotalMilliseconds / 1000f;
        }
    }

    private class UnityStopwatch : Stopwatch
    {
        private float m_StartTime;
        private WatchType m_Type;

        public UnityStopwatch(WatchType type)
        {
            m_Type = type;
            Reset();
        }

        public override void Reset()
        {
            m_StartTime = GetCurrentTime(m_Type);
        }

        public override void Stop()
        {
            TotalMilliseconds = (GetCurrentTime(m_Type) - m_StartTime) * 1000f;
            TotalSeconds = TotalMilliseconds / 1000f;
        }

        private static float GetCurrentTime(WatchType type)
        {
            switch (type)
            {
                case WatchType.UnityTime: return Time.time;
                case WatchType.RealTime: return Time.realtimeSinceStartup;
                default: return 0;
            }
        }
    }
}
