using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 控制小数点位数
    /// </summary>
    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var temp = DataUtil.ToValue<decimal>(value);
            return string.Format("{0:N1}", temp);
        }

        public readonly static DecimalConverter Instance = new DecimalConverter();
    }
}