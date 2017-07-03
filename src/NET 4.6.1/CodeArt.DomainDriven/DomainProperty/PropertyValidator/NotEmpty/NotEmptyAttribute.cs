using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public class NotEmptyAttribute : PropertyValidatorAttribute
    {
        public NotEmptyAttribute()
        {
        }

        public override IPropertyValidator CreateValidator()
        {
            return NotEmptyValidator.Instance;
        }

    }
}