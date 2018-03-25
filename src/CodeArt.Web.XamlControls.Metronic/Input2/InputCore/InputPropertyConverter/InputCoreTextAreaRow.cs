using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    [SafeAccess]
    internal class InputCoreTextAreaRow : InputPropertyConverter
    {
        /// <summary>
        /// 将绑定源的值转换为需要绑定的目标格式值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public override object Convert(object value, object parameter)
        {
            var input = FindInput(parameter);
            var textarea = input as Textarea;
            return textarea.Rows;
        }


        public static InputCoreTextAreaRow Instance = new InputCoreTextAreaRow();

    }
}