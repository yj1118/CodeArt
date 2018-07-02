using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.WPF
{
    //public static class KeyboardDetector
    //{
    //    public static bool ExsitPhysicalKeyboard { get; private set; }

    //    static KeyboardDetector()
    //    {
    //        ExsitPhysicalKeyboard = FindPhysicalKeyboard();
    //    }

    //    private static bool FindPhysicalKeyboard()
    //    {
    //        const string keywordEn1 = "keyboard";
    //        const string keywordEn2 = "Keyboard";
    //        const string keywordCN = "键盘";

    //        var hardInfo = GetAllDeviceString();

    //        if (hardInfo.Contains(keywordEn1) ||
    //            hardInfo.Contains(keywordEn2) ||
    //            hardInfo.Contains(keywordCN))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    public static string GetAllDeviceString()
    //    {
    //        StringBuilder sbDevHst = new StringBuilder();
    //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
    //        foreach (ManagementObject mgt in searcher.Get())
    //        {
    //            sbDevHst.AppendLine(Convert.ToString(mgt["Name"]));
    //            sbDevHst.AppendLine("");
    //        }

    //        // 返回计算机所有硬件信息的字符串
    //        return sbDevHst.ToString();//获取的字符串  
    //    }

    //}
}
