using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class OptionsCodeConverter : IValueConverter
    {
        public object Convert(object value, object parameter)
        {
            var options = value as UIElementCollection;
            if (options.Count == 0) return "[]";
            StringBuilder code = new StringBuilder("[");
            foreach(Option option in options)
            {
                code.Append("{");
                code.AppendFormat("value:'{0}',",option.Value);
                code.AppendFormat("text:'{0}'", option.Text);
                code.Append("},");
            }
            code.Append("]");
            return code.ToString();
        }


        public readonly static OptionsCodeConverter Instance = new OptionsCodeConverter();
    }
}