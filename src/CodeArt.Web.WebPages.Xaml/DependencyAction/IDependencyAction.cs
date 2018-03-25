using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDependencyAction
    {
        /// <summary>
        /// 行为名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 拥有该行为的类型
        /// </summary>
        Type OwnerType { get;}

        /// <summary>
        /// 是否允许客户端访问
        /// </summary>
        bool AllowClientAccess { get; }

        /// <summary>
        /// 默认元数据
        /// </summary>
        ActionMetadata DefaultMetadata { get; }

    }
}
