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
    public class UserProfileStyleConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var e = RenderContext.Current.Target as ITemplateCell;
            var file = e.BelongTemplate.GetFile("/assets/custom/images/user_profile_bg.jpg");
            return string.Format("background: url({0}); background-size: cover;", file.VirtualPath);
        }

        public readonly static UserProfileStyleConverter Instance = new UserProfileStyleConverter();
    }
}