using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 类型转换器，用于将字符串文本转换为特定的类型
    /// </summary>
    public interface ITypeConverter
    {
         object ConvertTo(string value, Type destinationType);
    }
}
