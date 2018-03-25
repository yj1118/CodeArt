using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CodeArt.AOP
{
    /// <summary>
    /// 配置节点未定义属性
    /// </summary>
    public class UnityException : Exception
    {
        public UnityException()
            : base()
        {
        }

        public UnityException(String message)
            : base(message)
        {
        }
    }
}
