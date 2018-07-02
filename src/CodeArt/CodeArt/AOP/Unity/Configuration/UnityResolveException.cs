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
    public class UnityResolveException : Exception
    {
        public UnityResolveException()
            : base()
        {
        }

        public UnityResolveException(string message)
            : base(message)
        {
        }

        public UnityResolveException(UnityResolveArgument arg)
            : base(String.Format("参数{0}指定的值{1}错误！", arg.Name, arg.Value))
        {
        }

    }
}
