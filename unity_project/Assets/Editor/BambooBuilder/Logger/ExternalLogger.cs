using System;
using System.Text;
using UnityEngine;
using Windows;

public class ExternalLogger : IDisposable
{
    public static int Indent = 0;
    private readonly ConsoleWindow m_Console;

    public ExternalLogger()
    {
        m_Console = new ConsoleWindow();
        m_Console.Initialize();

        Application.logMessageReceivedThreaded += OnLogReceived;
    }

    public void Dispose()
    {
        Application.logMessageReceivedThreaded -= OnLogReceived;
        m_Console.Shutdown();
    }

    private void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        Print(condition);

        if (type == LogType.Error || type == LogType.Exception)
        {
            Print(stackTrace);
        }
    }

    private void Print(string str)
    {
        str = FormatString(str);
        if (str.EndsWith("\r") == true)
        {
            m_Console.Write(str);
        }
        else
        {
            m_Console.WriteLine(str);
        }
    }

    private static string FormatString(string str)
    {
        var sb = new StringBuilder();
        string firstIndent = GetBlankSpace(Indent * 2);
        string lineIndent = GetBlankSpace(Indent * 2 + 10);
        sb.Append("  [Unity] ");
        sb.Append(firstIndent);
        sb.Append(str.Replace("\n", "\n" + lineIndent));
        return sb.ToString();
    }

    private static string GetBlankSpace(int count) => new string(' ', count);
}