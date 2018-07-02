using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeArt.WPF.AppProcess
{
    internal static class WinApi
    {
        // 3=最大化窗口
        internal const int SW_SHOWMAXIMIZED = 3;

        [DllImport("User32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}