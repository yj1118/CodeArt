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
    public class TextBoxRuleCodeConverter : RuleCodeConverter
    {

        protected override void FillRulesCode(object d, StringBuilder code)
        {
            var tb = d as TextBox;
            if (tb.Required) code.Append("required:true,");
            if (tb.Type.SequenceEqual("email")) code.Append("email:true,");
            if (!string.IsNullOrEmpty(tb.EqualToPrev)) code.Append("equalToPrev:true,");
            if (tb.MinLength > 0 && tb.MaxLength > 0)
            {
                code.AppendFormat("rangelength:[{0},{1}],", tb.MinLength, tb.MaxLength);
            }
            else
            {
                if (tb.MinLength > 0) code.AppendFormat("minlength:{0},", tb.MinLength);
                if (tb.MaxLength > 0) code.AppendFormat("maxlength:{0},", tb.MaxLength);
            }
        }

        protected override void FillMessagesCode(object d, StringBuilder code)
        {
            var tb = d as TextBox;
            var label = tb.Label;
            if (tb.Required) code.AppendFormat("required:'{0}',", string.Format(Strings.FieldRequired, label));
            if (tb.Type.SequenceEqual("email")) code.AppendFormat("email:'{0}',", Strings.PleaseEnterValidEmail);
            if (!string.IsNullOrEmpty(tb.EqualToPrev)) code.AppendFormat("equalToPrev:'{0}',", tb.EqualToPrev);
            if (tb.MinLength > 0 && tb.MaxLength > 0)
            {
                code.AppendFormat("rangelength:'{0}',", string.Format(Strings.FieldRange, label, tb.MinLength, tb.MaxLength));
            }
            else
            {
                if (tb.MinLength > 0) code.AppendFormat("minlength:'{0}',", string.Format(Strings.FieldMinLength, label, tb.MinLength));
                if (tb.MaxLength > 0) code.AppendFormat("maxlength:'{0}',", string.Format(Strings.FieldMaxLength, label, tb.MaxLength));
            }
        }

        public readonly static TextBoxRuleCodeConverter Instance = new TextBoxRuleCodeConverter();
    }
}