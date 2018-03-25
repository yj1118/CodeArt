using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public class EmailAttribute : PropertyValidatorAttribute
    {
        public EmailAttribute()
        {
        }

        public override IPropertyValidator CreateValidator()
        {
            return EmailValidator.Instance;
        }

    }
}