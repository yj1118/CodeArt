using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using CodeArt.Runtime;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 指示对象是基于appSession的数据访问,不同的用户的数据是独享的
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class AppSessionAccessAttribute : Attribute
    {
        public AppSessionAccessAttribute()
        {
        }

        #region 静态成员

        /// <summary>
        /// 检查类型是否为并发访问安全的
        /// </summary>
        /// <param name="type"></param>
        public static void CheckUp(Type type)
        {
            var access = AttributeUtil.GetAttribute<AppSessionAccessAttribute>(type);
            if (access == null)
                throw new TypeUnAppSessionAccessException(type);
        }

        public static void CheckUp(object obj)
        {
            CheckUp(obj.GetType());
        }

        public static bool IsDefined(Type type)
        {
            return AttributeUtil.GetAttribute<AppSessionAccessAttribute>(type) != null;
        }

        public static bool IsDefined(object obj)
        {
            return AttributeUtil.GetAttribute<AppSessionAccessAttribute>(obj.GetType()) != null;
        }

        #endregion
    }
}
