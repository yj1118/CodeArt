using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt
{
    public static class LanguageUtil
    {
        /// <summary>
        /// 用该方法可以创建对应<paramref name="language"/>的唯一CultureInfo对象，节省内存开销，返回的CultureInfo是只读的
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static CultureInfo GetCulture(string language)
        {
            return _getCulture(language);
        }

        private static Func<string, CultureInfo> _getCulture = LazyIndexer.Init<string, CultureInfo>((language) =>
        {
            var culture = new CultureInfo(language);
            return CultureInfo.ReadOnly(culture);
        });


        private static Func<string, Func<string, string>> _getLanguageString = LazyIndexer.Init<string, Func<string, string>>((language) =>
        {
            return LazyIndexer.Init<string, string>((key) =>
            {
                var attrs = LanguageAttribute.GetAll();
                var ci = LanguageUtil.GetCulture(language);
                foreach (var attr in attrs)
                {
                    var assembly = attr.Assembly;
                    var manager = new ResourceManager(string.Format("{0}.Strings", assembly.GetName().Name), assembly);
                    var value = manager.GetString(key, ci);
                    if (value != null) return value;
                }
                return string.Empty;
            });
        });

        /// <summary>
        /// 根据当前会话的语言设置，查找多语言文本
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string language, string key)
        {
            return _getLanguageString(language)(key);
        }

    }
}
