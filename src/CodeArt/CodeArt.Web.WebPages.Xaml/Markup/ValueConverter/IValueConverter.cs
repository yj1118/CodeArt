using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 值转换器，用于在绑定过程中将绑定源的值转换为需要绑定的目标格式值，反之亦然
    /// </summary>
    public interface IValueConverter
    {

        /// <summary>
        /// 将绑定源的值转换为需要绑定的目标格式值
        /// </summary>
        /// <param name="value">绑定源生成的值</param>
        /// <param name="parameter">要使用的转换器参数</param>
        /// <returns></returns>
        object Convert(object value, object parameter);

        ///// <summary>
        ///// 将绑定源的值转换为需要绑定的目标格式值
        ///// </summary>
        ///// <param name="value">绑定源生成的值</param>
        ///// <param name="targetType">绑定目标属性的类型</param>
        ///// <param name="parameter">要使用的转换器参数</param>
        ///// <returns></returns>
        //object Convert(object value, Type targetType, object parameter);

        /// <summary>
        /// 将绑定的格值转换为绑定源格式的值
        /// </summary>
        /// <param name="value">绑定目标生成的值</param>
        /// <param name="targetType">要转换到的类型</param>
        /// <param name="parameter">要使用的转换器参数</param>
        /// <returns></returns>
        //object ConvertBack(object value, Type targetType, object parameter);
    }
}
