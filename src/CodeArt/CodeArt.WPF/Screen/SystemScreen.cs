using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace CodeArt.WPF.Screen
{
    public static class SystemScreen
    {
        /// <summary>
        /// 以逻辑像素为单位获取屏幕区域大小，自动根据DPI计算出逻辑分辨率,如果有多个屏幕，那么使用的是屏幕总计宽度和高度
        /// </summary>
        public static readonly Rectangle LogicArea;
        public static readonly Rectangle PhysicalArea;

        public static readonly Rectangle LogicWorkArea;

        //public Rectangle LogicTaskbarArea;

        public static readonly ScreenDPI DPI;

        static SystemScreen()
        {
            DPI = GetDPI();
            LogicArea = GetLogicArea();
            LogicWorkArea = GetLogicWorkArea();
            PhysicalArea = GetPhysicalArea();
        }

        /// <summary>
        /// 以逻辑像素为单位获取屏幕区域大小，自动根据DPI计算出逻辑分辨率
        /// </summary>
        /// <returns></returns>
        private static Rectangle GetLogicArea()
        {
            Rectangle rc = SystemInformation.VirtualScreen;
            var dpi = GetDPI();
            rc.Width = (int)(rc.Width / dpi.ScalingX);
            rc.Height = (int)(rc.Height / dpi.ScalingY);
            return rc;
        }

        private static Rectangle GetPhysicalArea()
        {
            return SystemInformation.VirtualScreen;
        }

        private static Rectangle GetLogicWorkArea()
        {
            Rectangle rc = SystemInformation.WorkingArea;
            var dpi = GetDPI();
            rc.Width = (int)(rc.Width / dpi.ScalingX);
            rc.Height = (int)(rc.Height / dpi.ScalingY);
            return rc;
        }

        /// <summary>
        /// 获取系统当前设置的DPI信息
        /// </summary>
        /// <returns></returns>
        private static ScreenDPI GetDPI()
        {
            ScreenDPI dpi = new ScreenDPI();
            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpi.X = graphics.DpiX;
                dpi.Y = graphics.DpiY;
            }
            return dpi;
        }

        #region 多屏幕处理

        public static Rectangle GetLogicArea(Window window)
        {
            var screen = window.GetScreen();
            Rectangle rc = screen.Bounds;
            var dpi = GetDPI();
            rc.Width = (int)(rc.Width / dpi.ScalingX);
            rc.Height = (int)(rc.Height / dpi.ScalingY);
            return rc;
        }

        /// <summary>
        /// 像素宽转逻辑宽
        /// </summary>
        /// <param name="physicalWidth"></param>
        /// <returns></returns>
        public static int GetLogicWidth(int physicalWidth)
        {
            return (int)(physicalWidth / DPI.ScalingX);
        }

        /// <summary>
        /// 像素高转逻辑高
        /// </summary>
        /// <param name="physicalHeight"></param>
        /// <returns></returns>
        public static int GetLogicHeight(int physicalHeight)
        {
            return (int)(physicalHeight / DPI.ScalingY);
        }

        /// <summary>
        /// 逻辑宽转像素宽
        /// </summary>
        /// <param name="logicWidth"></param>
        /// <returns></returns>
        public static int GetPhysicalWidth(int logicWidth)
        {
            return (int)(logicWidth * DPI.ScalingX);
        }

        /// <summary>
        /// 逻辑高转像素高
        /// </summary>
        /// <param name="logicHeight"></param>
        /// <returns></returns>
        public static int GetPhysicalHeight(int logicHeight)
        {
            return (int)(logicHeight * DPI.ScalingY);
        }

        #endregion

    }
}
