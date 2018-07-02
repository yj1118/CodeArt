using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Web;

using CodeArt.Util;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 指示模板代码的来源，格式：
    /// 以反斜杠/开头，代表是站点虚拟路径
    /// 带由正斜杠\代表的是物理文件路径
    /// 以上两种都不满足，则为本程序集资源路径：resourceName,assemblyName
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class TemplateCodeAttribute : Attribute
    {
        /// <summary>
        /// 模板属性的名称
        /// </summary>
        public string TemplatePropertyName
        {
            get;
            private set;
        }

        public string Url
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName">模板对应的属性名称</param>
        /// <param name="url"></param>
        public TemplateCodeAttribute(string propertyName, string url)
        {
            this.TemplatePropertyName = propertyName;
            this.Url = url;
        }

        public string GetXaml()
        {
            if(this.Url.StartsWith("/"))
            {
                //相对路径
                var path = this.Url.Substring(1);
                var fileName =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.Replace("/", "\\"));
                return File.ReadAllText(fileName);
            }
            else if(this.Url.IndexOf("\\") > -1)
            {
                //文件路径
                return File.ReadAllText(this.Url);
            }
            else
            {
                //程序集资源
                return AssemblyResource.LoadText(this.Url);
            }
        }

        public static TemplateCodeAttribute GetAttribute(Type objType,string templatePropertyName)
        {
            var attrs = objType.GetCustomAttributes<TemplateCodeAttribute>(true);
            if (attrs == null) return null;
            foreach(var item in attrs)
            {
                if (item.TemplatePropertyName.Equals(templatePropertyName, StringComparison.OrdinalIgnoreCase)) return item;
            }
            return null;
        }
    }

}
