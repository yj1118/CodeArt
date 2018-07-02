using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IDependencyActionPreCallEventArgs
    {
        /// <summary>
        /// 是否允许执行行为
        /// </summary>
        bool Allow { get; set; }

        /// <summary>
        /// 获取或设置返回值
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// 行为需要的参数
        /// </summary>
        object[] Arguments { get; }

        DependencyAction Action { get; }
    }
}