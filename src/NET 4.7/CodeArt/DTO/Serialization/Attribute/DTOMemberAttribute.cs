using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CodeArt.DTO
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DTOMemberAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public DTOMemberType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为文件流类型
        /// </summary>
        public bool IsBlob
        {
            get;
            private set;
        }

        public DTOMemberAttribute(string name, DTOMemberType type, bool isBlob)
        {
            this.Name = name;
            this.Type = type;
            this.IsBlob = isBlob;
        }

        public DTOMemberAttribute(string name, DTOMemberType type)
            : this(name, type, false)
        {
        }

        public DTOMemberAttribute()
            : this(null, DTOMemberType.General, false)
        {
        }

        public DTOMemberAttribute(string name)
            : this(name, DTOMemberType.General, false)
        {
        }

        public DTOMemberAttribute(DTOMemberType type)
            : this(null, type, false)
        {
        }

        /// <summary>
        /// 获取成员定义的<see cref="DTOMemberAttribute"/>信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static DTOMemberAttribute GetAttribute(MemberInfo memberInfo)
        {
            object[] attributes = memberInfo.GetCustomAttributes(typeof(DTOMemberAttribute), true);
            return attributes.Length > 0 ? attributes[0] as DTOMemberAttribute : null;
        }
    }


    public enum DTOMemberType
    {
        /// <summary>
        /// 参数
        /// </summary>
        Parameter = 1,
        /// <summary>
        /// 返回值
        /// </summary>
        ReturnValue = 2,
        /// <summary>
        /// 通用
        /// </summary>
        General = 3
    }

}
