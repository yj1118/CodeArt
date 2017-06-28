using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DTO
{
    /// <summary>
    /// 代表接受参数的定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DTOParameterAttribute : DTOMemberAttribute
    {
        public DTOParameterAttribute(string name)
            : base(name, DTOMemberType.Parameter, false)
        {
        }

        public DTOParameterAttribute(string name,bool isBlob)
            : base(name, DTOMemberType.Parameter, isBlob)
        {
        }

        public DTOParameterAttribute()
            : this(null)
        {
        }
    }
}
