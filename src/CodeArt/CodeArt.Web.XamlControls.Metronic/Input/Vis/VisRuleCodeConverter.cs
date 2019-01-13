using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.Web.WebPages.Xaml;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Concurrent;
using CodeArt.Web.XamlControls.Metronic;

namespace CodeArt.Web.XamlControls.Metronic
{
    public class VisRuleCodeConverter : RuleCodeConverter
    {

        protected override void FillRulesCode(object d, StringBuilder code)
        {
        }

        protected override void FillMessagesCode(object d, StringBuilder code)
        {
        }

        public readonly static VisRuleCodeConverter Instance = new VisRuleCodeConverter();
    }
}