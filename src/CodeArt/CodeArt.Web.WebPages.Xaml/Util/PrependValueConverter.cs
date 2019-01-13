using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 将参数放在value前面的前置转换器
    /// </summary>
    public class PrependValueConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var pre = parameter as string;
            return string.Format("{0}{1}", pre, value);
        }

        public readonly static PrependValueConverter Instance = new PrependValueConverter();
    }
}