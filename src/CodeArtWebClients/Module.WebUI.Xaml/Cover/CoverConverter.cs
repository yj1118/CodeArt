using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages.Xaml.Markup;

using Module.WebUI;

namespace Module.WebUI.Xaml
{
    [SafeAccess]
    internal class CoverConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var coverId = Guid.Parse(value.ToString());
            var range = parameter == null ? string.Empty : parameter.ToString();
            int width = 0;
            int height = 0;
            if (!string.IsNullOrEmpty(range))
            {
                string[] temp = range.Split('|');
                if (temp.Length == 2)
                {
                    width = int.Parse(temp[0]);
                    height = int.Parse(temp[1]);
                }
            }

            return ImageUtil.GetCoverUrl(coverId, width, height);
        }

        public static readonly CoverConverter Instance = new CoverConverter();
    }
}
