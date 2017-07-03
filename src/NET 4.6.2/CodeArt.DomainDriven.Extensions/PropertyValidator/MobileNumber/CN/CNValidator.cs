using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    public static partial class MobileNumber
    {
        /// <summary>
        /// 中国地区手机格式的验证
        /// </summary>
        [SafeAccess]
        public class CNValidator : PropertyValidator<string>
        {
            private CNValidator()
            {
            }

            protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
            {
                if (string.IsNullOrEmpty(propertyValue)) return; //是否能为空的验证由别的验证器处理

                if (!IsMatch(propertyValue))
                    result.AddError(property.Name, "MobileNumber", string.Format(Strings.IncorrectMobileNumberFormat, property.Name));
            }

            private static RegexPool _regex = new RegexPool(@"^1[34578]\d{9}$", RegexOptions.IgnoreCase);

            public static readonly CNValidator Instance = new CNValidator();

            public static bool IsMatch(string input)
            {
                return _regex.IsMatch(input);
            }

        }
    }
}
