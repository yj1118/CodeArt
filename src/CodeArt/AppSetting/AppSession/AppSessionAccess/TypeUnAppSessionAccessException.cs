using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 类型的数据不是基于appSession的
    /// </summary>
    public class TypeUnAppSessionAccessException : Exception
    {
        public TypeUnAppSessionAccessException()
            : base()
        {
        }

        public TypeUnAppSessionAccessException(string message)
            : base(message)
        {
        }


        public TypeUnAppSessionAccessException(Type objType)
            : base(string.Format(Strings.TypeUnsafeConcurrentAccess, objType.FullName))
        {
        }
    }
}
