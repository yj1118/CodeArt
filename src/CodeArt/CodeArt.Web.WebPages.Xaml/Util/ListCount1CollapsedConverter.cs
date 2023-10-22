using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 当数量为1时，隐藏
    /// </summary>
    public class ListCount1CollapsedConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var list = value as IEnumerable;
            int index = 0;
            foreach(var t in list)
            {
                index++;
                if(index > 1) return Visibility.Visible;
            }
            return index == 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        public readonly static ListCount1CollapsedConverter Instance = new ListCount1CollapsedConverter();
    }
}