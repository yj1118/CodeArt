using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    [SafeAccess]
    public class MobileNumberValidator : PropertyValidator<string>
    {
        private MobileNumberValidator()
        {
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
        {
            if (string.IsNullOrEmpty(propertyValue)) return; //是否能为空的验证由别的验证器处理

            if (!IsMatch(propertyValue))
                result.AddError(property.Name, "MobileNumber", string.Format(Strings.IncorrectMobileNumberFormat, property.Call));
        }

        private static RegexPool _regex = new RegexPool(@"^\d{3,}$", RegexOptions.IgnoreCase);

        public static readonly MobileNumberValidator Instance = new MobileNumberValidator();

        public static bool IsMatch(string input)
        {
            return _regex.IsMatch(input);
        }

    }
}
