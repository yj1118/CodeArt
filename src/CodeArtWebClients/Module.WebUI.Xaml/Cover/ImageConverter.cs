using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace Module.WebUI.Xaml
{
    [SafeAccess]
    internal class ImageConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var key = value.ToString();
            var range = parameter == null ? string.Empty : parameter.ToString();
            int width = 0;
            int height = 0;
            int cutType = 1;
            if (!string.IsNullOrEmpty(range))
            {
                string[] temp = range.Split('|');
                if (temp.Length == 3)
                {
                    width = int.Parse(temp[0]);
                    height = int.Parse(temp[1]);
                    cutType = int.Parse(temp[2]);
                }
            }

            return ImageUtil.GetDynamicUrl(key, width, height, cutType);
        }

        public static readonly ImageConverter Instance = new ImageConverter();
    }
}
