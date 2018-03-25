using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ComponentLoaderAttribute : Attribute
    {
        public Type LoaderType { get; private set; }
        public ComponentLoaderAttribute(Type loaderType)
        {
            this.LoaderType = loaderType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ComponentLoaderAttribute GetAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ComponentLoaderAttribute), true);
            return attributes.Length > 0 ? attributes[0] as ComponentLoaderAttribute : null;
        }

    }
}
