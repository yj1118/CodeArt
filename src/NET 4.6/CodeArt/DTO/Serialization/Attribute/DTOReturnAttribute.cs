using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DTO
{
    /// <summary>
    /// 代表返回值的定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DTOReturnAttribute : DTOMemberAttribute
    {
        public DTOReturnAttribute(string name)
            : base(name)
        {
        }


        public DTOReturnAttribute()
        {
        }


        public new static DTOReturnAttribute GetAttribute(MemberInfo memberInfo)
        {
            object[] attributes = memberInfo.GetCustomAttributes(typeof(DTOReturnAttribute), true);
            return attributes.Length > 0 ? attributes[0] as DTOReturnAttribute : null;
        }

    }
}
