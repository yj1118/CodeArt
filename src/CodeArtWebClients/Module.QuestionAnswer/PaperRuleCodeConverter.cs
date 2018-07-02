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

namespace Module.QuestionAnswer
{
    public class PaperRuleCodeConverter : RuleCodeConverter
    {

        protected override void FillRulesCode(object d, StringBuilder code)
        {
            code.Append("paperValidate:true,");
        }

        protected override void FillMessagesCode(object d, StringBuilder code)
        {
            var target = d as Paper;
            var label = target.Label;
            code.AppendFormat("paperValidate:'{0}',", string.Format(Strings.PaperError, label));
        }

        public readonly static PaperRuleCodeConverter Instance = new PaperRuleCodeConverter();
    }
}