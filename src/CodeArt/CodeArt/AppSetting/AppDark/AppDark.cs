using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.AppSetting
{
    public static class AppDark
    {
        private static IAppDarkFilter _filter;

        public static void RegisterFilter(IAppDarkFilter filter)
        {
            _filter = filter;
        }

        /// <summary>
        /// 判断当前访问者是否在灰度中
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool In(DTObject arg)
        {
            if (_filter == null) return false;
            return _filter.InDark(arg);
        }
    }
}
