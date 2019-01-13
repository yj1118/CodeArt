using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;
using System.ComponentModel;

using CodeArt.Runtime;

namespace CodeArt.AppSetting
{
    /// <summary>
    /// 每次会话被释放之前调用该特性标记的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class PreAppSessionEndAttribute : Attribute
    {
        public Type Type
        {
            get;
            private set;
        }

        public string MethodName
        {
            get;
            private set;
        }

        /// <summary>
        /// 每次会话被释放之前调用该特性标记的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="runPriority">系统内部可以设置运行优先级</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PreAppSessionEndAttribute(Type type, string methodName)
        {
            this.Type = type;
            this.MethodName = methodName;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            var method = this.Type.ResolveMethod(MethodName);
            ArgumentAssert.IsNotNull(method, MethodName);
            if (!method.IsStatic)
                throw new AppSettingException(string.Format("PreAppSessionEndAttribute 调用的方法 {0}.{1}必须是静态的", this.Type.FullName, this.MethodName));
            method.Invoke(null, null);
        }
    }
}
