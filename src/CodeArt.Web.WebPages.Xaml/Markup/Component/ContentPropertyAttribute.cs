using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class ContentPropertyAttribute : Attribute
    {
        public string Name { get; private set; }

        public ContentPropertyAttribute(string name)
        {
            this.Name = name;
        }

        #region 辅助

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ContentPropertyAttribute GetAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ContentPropertyAttribute), true);
            return attributes.Length > 0 ? attributes[0] as ContentPropertyAttribute : null;
        }

        #endregion

    }
}
