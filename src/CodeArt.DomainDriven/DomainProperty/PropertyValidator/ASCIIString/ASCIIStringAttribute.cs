using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

using CodeArt.Runtime;
using CodeArt.Runtime.IL;

namespace CodeArt.DomainDriven
{
    public class ASCIIStringAttribute : PropertyValidatorAttribute
    {
        public ASCIIStringAttribute()
        {
        }

        public override IPropertyValidator CreateValidator()
        {
            return new ASCIIStringValidator();
        }

    }
}