using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 仅验证是否全为数字，真实验证交由短信机制
    /// </summary>
    public class MobileNumberAttribute : PropertyValidatorAttribute
    {
        public override IPropertyValidator CreateValidator()
        {
            return MobileNumberValidator.Instance;
        }
    }
}