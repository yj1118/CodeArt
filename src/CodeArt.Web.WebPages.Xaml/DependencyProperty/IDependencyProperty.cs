using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IDependencyProperty
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 属性的类型
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// 拥有该属性的类型
        /// </summary>
        Type OwnerType { get; }

        /// <summary>
        /// 属性的元数据
        /// </summary>
        PropertyMetadata DefaultMetadata { get; }
    }
}
