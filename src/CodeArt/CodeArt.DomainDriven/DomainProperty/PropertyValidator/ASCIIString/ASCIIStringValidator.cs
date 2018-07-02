using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 验证属性为ASCII字符串
    /// </summary>
    public class ASCIIStringValidator : PropertyListabelValidator<string>
    {
        public ASCIIStringValidator() { }

        protected override void Validate(DomainObject domainObject, DomainProperty property, string propertyValue, ValidationResult result)
        {
            if (propertyValue != null)
            {
                if (!propertyValue.IsASCII())
                {
                    result.AddError(property.Name, "NotASCII", string.Format(Strings.NotASCII, propertyValue));
                }
            }
        }
    }
}
