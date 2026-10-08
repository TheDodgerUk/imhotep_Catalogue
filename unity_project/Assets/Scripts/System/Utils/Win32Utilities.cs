using System.Runtime.InteropServices;

namespace Utils
{
    public static class Win32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteFile(string path);

        public static string ExtendPath(string path) => @"\\?\" + path;
    }
}