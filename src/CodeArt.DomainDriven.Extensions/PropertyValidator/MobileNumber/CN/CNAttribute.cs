using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 目前仅支持中国地区的手机号码验证，以后补充更多地区
    /// </summary>
    public static partial class MobileNumber
    {
        public class CNAttribute : PropertyValidatorAttribute
        {
            public CNAttribute()
            {
            }

            public override IPropertyValidator CreateValidator()
            {
                return CNValidator.Instance;
            }
        }
    }
}