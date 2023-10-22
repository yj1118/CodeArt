using System;
using System.Collections.Generic;
using System.Reflection;
using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt
{
    /// <summary>
    /// 表示参与语言选项的程序集
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class LanguageAttribute : Attribute
    {

        public LanguageAttribute(string assemblyName)
        {
            ArgumentAssert.IsNotNullOrEmpty(assemblyName, "assemblyName");

            this.AssemblyName = assemblyName;
        }

        /// <summary>
        /// 获取或设置与此特性关联的程序集的名称。
        /// </summary>
        public string AssemblyName
        {
            get;
            private set;
        }

        public Assembly Assembly
        {
            get
            {
                return AssemblyUtil.Get(this.AssemblyName);
            }
        }

        #region 静态方法

        private static IEnumerable<LanguageAttribute> _data = null;
        private static object syncObject = new object();

        internal static IEnumerable<LanguageAttribute> GetAll()
        {
            if(_data == null)
            {
                lock(syncObject)
                {
                    if (_data == null)
                    {
                        _data = AssemblyUtil.GetAttributes<LanguageAttribute>();
                    }
                }
            }
            return _data;
        }

        #endregion

    }
}
