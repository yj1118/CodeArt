using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeArt.WPF.Keyboard
{
    internal static class WinApi
    {
        internal const Int32 WM_LBUTTONDOWN = 513;
        internal const Int32 WM_LBUTTONUP = 514;
        internal const Int32 WM_SYSCOMMAND = 274;

        internal const UInt32 MK_LBUTTON = 1;
        internal const UInt32 SC_CLOSE = 61536;

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "PostMessage")]
        internal static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindWindowEx")]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    }
}