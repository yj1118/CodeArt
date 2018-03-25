using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class UserPhotoConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var v = value as string;
            if (!string.IsNullOrEmpty(v)) return v;
            var e = RenderContext.Current.Target as ITemplateCell;
            var file = e.BelongTemplate.GetFile("/assets/custom/images/photo.png");
            return file.VirtualPath;
        }

        public readonly static UserPhotoConverter Instance = new UserPhotoConverter();
    }
}