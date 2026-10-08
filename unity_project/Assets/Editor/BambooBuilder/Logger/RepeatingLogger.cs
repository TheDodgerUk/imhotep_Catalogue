using System.Threading;
using UnityEngine;

namespace BuildSystem
{
    public class RepeatingLogger
    {
        private ThreadFunnel m_Funnel;

        public RepeatingLogger()
        {
            m_Funnel = new ThreadFunnel();
        }

        public void Start(string message)
        {
            m_Funnel.Start(message);
            ThreadPool.QueueUserWorkItem(RepeatLog, m_Funnel);
        }

        public void Stop()
        {
            Debug.Log(m_Funnel.Message);
            m_Funnel.Run = false;
            m_Funnel = new ThreadFunnel();
        }

        private static void RepeatLog(object state)
        {
            var funnel = state as ThreadFunnel;
            var sb = new System.Text.StringBuilder();
            const int MaxDots = 10;
            int dots = 0;
            while (funnel.Run == true)
            {
                sb.Clear();
                sb.Append(funnel.Message);
                sb.Append('.', dots);
                sb.Append(' ', MaxDots - dots);
                sb.Append('\r');
                Debug.Log(sb.ToString());

                dots++;
                if (dots > MaxDots)
                {
                    dots = 0;
                }
                Thread.Sleep(500);
            }
        }

        private class ThreadFunnel
        {
            public volatile bool Run = true;
            public string Message = string.Empty;

            internal void Start(string message)
            {
                Message = message;
                Run = true;
            }
        }
    }
}
