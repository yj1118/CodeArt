using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DTO
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DTOReturnValueAttribute : DTOMemberAttribute
    {
        public DTOReturnValueAttribute(string name)
            : base(name, DTOMemberType.ReturnValue,false)
        {
        }

        public DTOReturnValueAttribute(string name, bool isBlob)
            : base(name, DTOMemberType.ReturnValue, isBlob)
        {
        }

        public DTOReturnValueAttribute()
            : this(null)
        {
        }


    }

}
