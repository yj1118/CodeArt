using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 邮箱格式验证
    /// </summary>
    [SafeAccess]
    public class EmailValidator : PropertyValidator<string>
    {
        private EmailValidator()
        {
        }

        protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
        {
            if (string.IsNullOrEmpty(propertyValue)) return; //是否能为空的验证由别的验证器处理

            if (!IsMatch(propertyValue))
                result.AddError(property.Name, "Email", string.Format(Strings.IncorrectEmailFormat, property.Call));
        }

        private static RegexPool _regex = new RegexPool(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.IgnoreCase);

        public static readonly EmailValidator Instance = new EmailValidator();

        public static bool IsMatch(string input)
        {
            return _regex.IsMatch(input);
        }

    }
}
