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
    public abstract class RuleCodeConverter : IValueConverter
    {

        public object Convert(object value, object parameter)
        {
            var d = RenderContext.Current.BelongTemplate.TemplateParent;
            using (var temp = StringPool.Borrow())
            {
                var code = temp.Item;
                code.Append("{rules:{");
                FillRulesCode(d, code);
                code.Append("},");
                code.Append("messages:{");
                FillMessagesCode(d, code);
                code.Append("}}");
                return code.ToString();
            }
        }

        protected abstract void FillRulesCode(object d, StringBuilder code);

        protected abstract void FillMessagesCode(object d, StringBuilder code);
    }
}