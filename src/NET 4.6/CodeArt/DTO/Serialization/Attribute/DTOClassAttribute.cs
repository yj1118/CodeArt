using System;
using System.Collections.Generic;
using System.Text;

namespace CodeArt.DTO
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = false)]
    public class DTOClassAttribute : Attribute
    {
        public DTOSerializableMode Mode
        {
            get;
            private set;
        }


        public DTOClassAttribute(DTOSerializableMode mode)
        {
            this.Mode = mode;
        }

        public DTOClassAttribute()
            : this(DTOSerializableMode.General)
        {
        }

        #region 辅助

        /// <summary>
        /// 获取类型定义的<see cref="DTOClassAttribute"/>信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DTOClassAttribute GetAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DTOClassAttribute), true);
            return attributes.Length > 0 ? attributes[0] as DTOClassAttribute : DTOClassAttribute.Default;
        }

        #endregion



        public static readonly DTOClassAttribute Default = new DTOClassAttribute();
    }


    public enum DTOSerializableMode
    {
        /// <summary>
        /// 常规模式
        /// </summary>
        General = 1,
        /// <summary>
        /// 函数模式,该模式表示对象有一部分属性用于接受DTO的值，另外一部分属性用于传递值到DTO中
        /// </summary>
        Function = 2
    }

}
