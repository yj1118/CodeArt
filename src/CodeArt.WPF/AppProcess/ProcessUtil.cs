using CodeArt.WPF.AppProcess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF
{
    public static class ProcessUtil
    {
        /// <summary>
        /// 获得相同程序的已经先运行了的进程
        /// </summary>
        /// <param name="appName">appName不包含扩展名</param>
        /// <param name="appPath">appPath是完整的路径名</param>
        /// <returns></returns>
        public static Process GetRanProcess(string appName, string appPath)
        {
            if(string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(appPath))
                return null;

            // 获取当前活动的进程
            Process currentProcess = Process.GetCurrentProcess();

            // 根据当前进程的进程名获得进程集合
            // 如果该程序运行，进程的数量大于1
            Process[] processcollection = Process.GetProcessesByName(appName);
            
            foreach (Process process in processcollection)
            {
                // 如果进程ID不等于当前运行进程的ID以及运行进程的文件路径等于当前进程的文件路径
                // 则说明同一个该程序已经运行了，此时将返回已经运行的进程
                if (process.Id != currentProcess.Id)
                {
                    if (appPath.Replace("/", "\\") == process.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 把已经运行了的进程带回到前台显示
        /// </summary>
        /// <param name="instance"></param>
        public static void ShowRanProcess(Process instance)
        {
            if (instance == null)
                return;

            // 显示窗口
            WinApi.ShowWindow(instance.MainWindowHandle, WinApi.SW_SHOWMAXIMIZED);

            // 把窗体放在前端
            WinApi.SetForegroundWindow(instance.MainWindowHandle);
        }

    }
}
