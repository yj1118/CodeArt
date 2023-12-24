using CodeArt.WPF.Keyboard;
using System;

namespace CodeArt.WPF
{
    public static class TabTipBoard
    {
        private static IntPtr _hTaptip = IntPtr.Zero;

        private const string ShellTray = "Shell_TrayWnd";
        private const string TrayNotify = "TrayNotifyWnd";
        private const string TabTip = "TIPBand";
        private const string TabTipWindow = "IPTip_Main_Window";

        static TabTipBoard()
        {
            IntPtr hWnd = WinApi.FindWindow(ShellTray, null);

            if (hWnd != IntPtr.Zero)
            {
                hWnd = WinApi.FindWindowEx(hWnd, IntPtr.Zero, TrayNotify, null);

                if (hWnd != IntPtr.Zero)
                {
                    _hTaptip = WinApi.FindWindowEx(hWnd, IntPtr.Zero, TabTip, null);
                }
            }
        }

        //关闭键盘
        public static void Close()
        {
            IntPtr hWnd = WinApi.FindWindow(TabTipWindow, null);

            if (hWnd != IntPtr.Zero)
            {
                WinApi.PostMessage(hWnd, WinApi.WM_SYSCOMMAND, WinApi.SC_CLOSE, 0);
            }
        }

        public static void ClickTabTip()
        {
            if (_hTaptip != IntPtr.Zero)
            {
                WinApi.PostMessage(_hTaptip, WinApi.WM_LBUTTONDOWN, WinApi.MK_LBUTTON, 65537);
                WinApi.PostMessage(_hTaptip, WinApi.WM_LBUTTONUP, 0, 65537);
            }
        }
    }


}