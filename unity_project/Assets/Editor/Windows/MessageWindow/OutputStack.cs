using System.Collections.Generic;
using UnityEngine;

namespace EditorTools.OutputStack
{
    [System.Serializable]
    public class OutputStack : System.IDisposable
    {
        public List<OutputMessage> Messages => m_Messages;

        private List<OutputMessage> m_Messages;
        private string m_Source;

        public OutputStack() : this(null) { }
        public OutputStack(string source)
        {
            m_Messages = new List<OutputMessage>();
            m_Source = source;
        }

        public string GetSource()
        {
            if (string.IsNullOrEmpty(m_Source) == false)
            {
                return m_Source;
            }
            return m_Messages.Find(e => e.HasSource)?.Source;
        }

        public void Clear() => m_Messages.Clear();

        public void AddWarning(string message, Object context) => Add(message, context, ErrorType.Warning);
        public void AddError(string message, Object context) => Add(message, context, ErrorType.Error);
        public void AddInfo(string message, Object context) => Add(message, context, ErrorType.Info);

        public void AddWarning(string message) => AddWarning(message, null);
        public void AddError(string message) => AddError(message, null);
        public void AddInfo(string message) => AddInfo(message, null);

        public void Add(OutputMessage message) => m_Messages.Add(message);

        public void Add(string message, Object context, ErrorType type)
        {
            Add(new OutputMessage(m_Source, message, type, context));
        }

        public void Dispose()
        {
            ShowIfNotEmpty();
        }

        public void ShowIfNotEmpty()
        {
            if (m_Messages.Count > 0)
            {
                OutputWindow.Show(this);
            }
        }

        public void SetActive()
        {
            Log.Active = this;
        }
    }

    public class Log
    {
        public static OutputStack Active { get; set; }
        public static void Clear() => Active = null;
    }
}
