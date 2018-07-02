using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class CropperDiskIdConverter : IValueConverter
    {
        /// <summary>
        /// 将绑定源的值转换为需要绑定的目标格式值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public object Convert(object value, object parameter)
        {
            return string.Format("disk_{0}", value != null ? value.ToString() : string.Empty);
        }


        public static CropperDiskIdConverter Instance = new CropperDiskIdConverter();

    }
}