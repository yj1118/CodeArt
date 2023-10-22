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
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class ProApplicationStartedAttribute : Attribute
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
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="runPriority">系统内部可以设置运行优先级</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ProApplicationStartedAttribute(Type type, string methodName)
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
                throw new AppSettingException(string.Format(Strings.ProApplicationStartedNoStatic, this.Type.FullName, this.MethodName));
            method.Invoke(null, null);
        }
    }
}
