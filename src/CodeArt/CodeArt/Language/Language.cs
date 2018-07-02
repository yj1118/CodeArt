using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Xml;

using CodeArt.DTO;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt
{
    public static class Language
    {
        /// <summary>
        /// 初始化语言选项
        /// </summary>
        public static void Init()
        {
            var language = LanguageProvider.CreateProvider().GetLanguage();
            CultureInfo ci = LanguageUtil.GetCulture(language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            AppSession.SetItem("language", ci);
        }

        private static object _syncObject = new object();

        /// <summary>
        /// 系统当前使用的语言
        /// </summary>
        public static CultureInfo Current
        {
            get
            {
                var current = AppSession.GetItem<CultureInfo>("language");
                if(current == null)
                {
                    lock(_syncObject)
                    {
                        current = AppSession.GetItem<CultureInfo>("language");
                        if (current == null)
                        {
                            Init();
                            current = AppSession.GetItem<CultureInfo>("language");
                        }
                    }
                }
                return current;
            }
        }

      
    }
}
