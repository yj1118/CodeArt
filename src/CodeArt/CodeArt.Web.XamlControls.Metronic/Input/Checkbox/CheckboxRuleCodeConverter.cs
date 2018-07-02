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
    public class CheckboxRuleCodeConverter : RuleCodeConverter
    {

        protected override void FillRulesCode(object d, StringBuilder code)
        {
            var target = d as Checkbox;
            if (target.Required) code.Append("required:true,");
        }

        protected override void FillMessagesCode(object d, StringBuilder code)
        {
            var target = d as Checkbox;
            var label = target.Label;
            if (target.Required) code.AppendFormat("required:'{0}',", string.Format(Strings.PleaseSelect, label));
        }

        public readonly static CheckboxRuleCodeConverter Instance = new CheckboxRuleCodeConverter();
    }
}