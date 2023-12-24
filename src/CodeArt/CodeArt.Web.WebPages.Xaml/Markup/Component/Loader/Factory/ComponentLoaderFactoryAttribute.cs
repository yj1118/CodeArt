using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ComponentLoaderFactoryAttribute : Attribute
    {
        public Type LoaderFactoryType { get; private set; }
        public ComponentLoaderFactoryAttribute(Type loaderFactoryType)
        {
            this.LoaderFactoryType = loaderFactoryType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ComponentLoaderFactoryAttribute GetAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ComponentLoaderFactoryAttribute), true);
            return attributes.Length > 0 ? attributes[0] as ComponentLoaderFactoryAttribute : null;
        }

    }
}
