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
        public bool Trim
        {
            get;
            set;
        }

        public NotEmptyAttribute()
        {
            this.Trim = true;
        }

        public NotEmptyAttribute(bool trim)
        {
            this.Trim = true;
        }

        public override IPropertyValidator CreateValidator()
        {
            return new NotEmptyValidator(this.Trim);
        }

    }
}