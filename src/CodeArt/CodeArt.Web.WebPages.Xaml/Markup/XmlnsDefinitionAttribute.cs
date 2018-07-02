using System;
using System.Collections.Generic;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class XmlnsDefinitionAttribute : Attribute
    {
        public XmlnsDefinitionAttribute(string xamlNamespace, string clrNamespace, string assemblyName)
        {
            ArgumentAssert.IsNotNullOrEmpty(xamlNamespace, "xamlNamespace");
            ArgumentAssert.IsNotNullOrEmpty(clrNamespace, "clrNamespace");
            ArgumentAssert.IsNotNullOrEmpty(assemblyName, "assemblyName");

            this.XamlNamespace = xamlNamespace;
            this.ClrNamespace = clrNamespace;
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

        /// <summary>
        /// 获取此特性中指定的 CLR 命名空间的字符串名称。 
        /// </summary>
        public string ClrNamespace
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取此特性中指定的 XAML 命名空间标识符。
        /// </summary>
        public string XamlNamespace
        {
            get;
            private set;
        }


        #region 静态方法

        private static MultiDictionary<string, XmlnsDefinitionAttribute> _global = null;
        private static object syncObject = new object();

        private static MultiDictionary<string, XmlnsDefinitionAttribute> GetGlobalDefinitions()
        {
            var attrs = AssemblyUtil.GetAttributes<XmlnsDefinitionAttribute>();
            MultiDictionary<string, XmlnsDefinitionAttribute> dic = new MultiDictionary<string, XmlnsDefinitionAttribute>(false);
            foreach (var attr in attrs)
            {
                dic.Add(attr.XamlNamespace, attr);
            }
            return dic;
        }


        private static Func<string, Func<string,Type>> _getComponentTypeMethod = LazyIndexer.Init<string, Func<string,Type>>((xamlNamespace) =>
        {
            return LazyIndexer.Init<string, Type>((localComponentName)=>
            {
                if(_global == null)
                {
                    lock(syncObject)
                    {
                        if (_global == null)
                            _global = GetGlobalDefinitions();
                    }
                }

                IList<XmlnsDefinitionAttribute> definitions = null;
                if (!_global.TryGetValue(xamlNamespace, out definitions)) return null;

                Type componentType = null;
                foreach (var defintion in definitions)
                {
                    componentType = Type.GetType(string.Format("{0}.{1},{2}", defintion.ClrNamespace, localComponentName, defintion.AssemblyName), false, true);
                    if (componentType != null) break;
                }
                return componentType;
            });
        });

        /// <summary>
        /// 通过xmlns定义，得到对象类型
        /// </summary>
        /// <param name="xamlNamespace"></param>
        /// <param name="localComponentName"></param>
        /// <returns></returns>
        internal static Type GetComponentType(string xamlNamespace, string localComponentName)
        {
            var getComponentType = _getComponentTypeMethod(xamlNamespace);
            if (getComponentType == null) return null;
            return getComponentType(localComponentName);
        }

        internal static Assembly GetAssembly(string xamlNamespace)
        {
            var attr = _global.GetValue((t) => t.XamlNamespace == xamlNamespace);
            return attr == null ? null : AssemblyUtil.Get(attr.AssemblyName); 
        }

        #endregion

    }
}
