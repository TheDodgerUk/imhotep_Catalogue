using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Windows
{
    public class ConsoleWindow
    {
        TextWriter m_StdOut;

        public void Initialize()
        {
            try
            {
                // Attach to any existing consoles we have
                // failing that, create a new one.
                if (!AttachConsole(0x0ffffffff))
                {
                    AllocConsole();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't attach console: \n" + e.Message);
            }

            try
            {
                IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
                var safeFileHandle = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdHandle, true);
                var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
                System.Text.Encoding encoding = System.Text.Encoding.ASCII;
                m_StdOut = new StreamWriter(fileStream, encoding)
                {
                    AutoFlush = true
                };
            }
            catch (Exception e)
            {
                Debug.Log("Couldn't redirect output: \n" + e.Message);
            }
        }

        public void Write(string str)
        {
            m_StdOut?.Write(str);
        }

        public void WriteLine(string str)
        {
            m_StdOut?.WriteLine(str);
        }

        public void Shutdown()
        {
            FreeConsole();
        }

        private const int STD_OUTPUT_HANDLE = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
    }
}