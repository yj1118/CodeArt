using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeArt.Log
{
    /// <summary>
    /// 标示对象信息不记录日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class NonLogAttribute : Attribute
    {
        public NonLogAttribute() { }

        #region 辅助

        /// <summary>
        /// 获取类型定义的<see cref="NonLogAttribute"/>信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefined(Type type)
        {
            return type.GetCustomAttributes(typeof(NonLogAttribute), true).Length > 0;
        }

        public static bool IsDefined(Exception ex)
        {
            if (IsDefined(ex.GetType())) return true;
            if (ex.Data.Contains(MarkecCode)) return true;
            return false;
        }

        public static void Define(Exception ex)
        {
            ex.Data.Add(NonLogAttribute.MarkecCode, true);
        }

        #endregion

        public const string MarkecCode = "NonLog";

    }
}