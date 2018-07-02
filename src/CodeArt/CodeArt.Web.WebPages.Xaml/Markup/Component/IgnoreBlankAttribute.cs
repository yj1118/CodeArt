using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 是否忽略转行节点
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class IgnoreBlankAttribute : Attribute
    {
        public bool Ignore { get; private set; }

        public IgnoreBlankAttribute(bool ignore = true)
        {
            this.Ignore = ignore;
        }

        #region 辅助

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IgnoreBlankAttribute GetAttribute(MemberInfo member)
        {
            var attr = member.GetCustomAttribute(typeof(IgnoreBlankAttribute), true) as IgnoreBlankAttribute;

            return attr ?? Default;
        }


        public static IgnoreBlankAttribute Default = new IgnoreBlankAttribute(true);


        #endregion

    }
}
