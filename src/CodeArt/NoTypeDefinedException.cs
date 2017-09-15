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
    /// 没有定义类型
    /// </summary>
    public class NoTypeDefinedException : TypeLoadException
    {
        public NoTypeDefinedException(Type type)
            : base(string.Format(Strings.NoTypeDefined, type.FullName))
        {

        }

        public NoTypeDefinedException(string typeName)
             : base(string.Format(Strings.NoTypeDefined, typeName))
        {

        }
    }
}
