using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    }
}
