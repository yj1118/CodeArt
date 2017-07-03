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
    public class NoPropertyDefinedException : InvalidCastException
    {
        public NoPropertyDefinedException(Type objType, string propertyName)
            : base(string.Format(Strings.NoPropertyDefined, propertyName, objType.FullName))
        {

        }
    }
}
