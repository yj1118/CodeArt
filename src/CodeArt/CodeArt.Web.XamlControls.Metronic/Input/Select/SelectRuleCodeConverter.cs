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
    public class SelectRuleCodeConverter : RuleCodeConverter
    {

        protected override void FillRulesCode(object d, StringBuilder code)
        {
            var target = d as Select;
            if (target.Required) code.Append("required:true,");
            if (target.MinLength > 0 && target.MaxLength > 0)
            {
                code.AppendFormat("rangelength:[{0},{1}],", target.MinLength, target.MaxLength);
            }
            else
            {
                if (target.MinLength > 0) code.AppendFormat("minlength:{0},", target.MinLength);
                if (target.MaxLength > 0) code.AppendFormat("maxlength:{0},", target.MaxLength);
            }
        }

        protected override void FillMessagesCode(object d, StringBuilder code)
        {
            var target = d as Select;
            var label = target.Label;
            if (target.Required) code.AppendFormat("required:'{0}',", string.Format(Strings.PleaseSelect, label));
            if (target.MinLength > 0 && target.MaxLength > 0)
            {
                code.AppendFormat("rangelength:'{0}',", string.Format(Strings.RangeSelectedCount, label, target.MinLength, target.MaxLength));
            }
            else
            {
                if (target.MinLength > 0) code.AppendFormat("minlength:'{0}',", string.Format(Strings.MinSelectedCountTip, label, target.MinLength));
                if (target.MaxLength > 0) code.AppendFormat("maxlength:'{0}',", string.Format(Strings.MaxSelectedCountTip, label, target.MaxLength));
            }
        }

        public readonly static SelectRuleCodeConverter Instance = new SelectRuleCodeConverter();
    }
}