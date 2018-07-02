using System;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.Concurrent;
using System.Globalization;

namespace CodeArt
{
    /// <summary>
    /// 通过配置文件配置的语言选项
    /// </summary>
    internal sealed class DefaultLanguageProvider : ILanguageProvider
    {
        public string GetLanguage()
        {
            return Configuration.Current.Language;
        }

        public static DefaultLanguageProvider Instance = new DefaultLanguageProvider();

       
    }
}
