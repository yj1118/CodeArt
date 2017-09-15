using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt
{
    /// <summary>
    /// 类型不匹配
    /// </summary>
    public class TypeMismatchException : InvalidCastException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType">实际类型</param>
        /// <param name="targetType">需要匹配的类型</param>
        public TypeMismatchException(Type objType, Type targetType)
            : base(string.Format(Strings.TypeMismatch, objType.FullName, targetType.FullName))
        {

        }
    }
}
