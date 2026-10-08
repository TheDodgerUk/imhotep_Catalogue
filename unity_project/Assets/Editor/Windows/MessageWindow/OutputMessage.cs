using UnityEngine;

namespace EditorTools.OutputStack
{
    [System.Serializable]
    public class OutputMessage
    {
        public bool HasSource => !string.IsNullOrEmpty(m_Source);

        public ErrorType Type => m_Type;
        public Object Context => m_Context;
        public string Message => m_Message;
        public string Source => m_Source;

        private ErrorType m_Type;
        private Object m_Context;
        private string m_Message;
        private string m_Source;

        public OutputMessage(string source, string message, ErrorType type, Object context)
        {
            m_Context = context;
            m_Message = message;
            m_Source = source;
            m_Type = type;
        }
    }

    public enum ErrorType { Info, Warning, Error };
}
