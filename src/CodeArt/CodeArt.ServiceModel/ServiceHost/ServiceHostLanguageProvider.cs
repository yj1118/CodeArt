using System;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

using CodeArt.Concurrent;
using System.Globalization;

namespace CodeArt.ServiceModel
{
    /// <summary>
    /// 基于ServiceHost提供的语言信息
    /// </summary>
    [SafeAccess]
    public sealed class ServiceHostLanguageProvider : ILanguageProvider
    {
        public string GetLanguage()
        {
            return AppContext.Language ?? Configuration.Current.Language;
        }

        public static ServiceHostLanguageProvider Instance = new ServiceHostLanguageProvider();

       
    }
}
