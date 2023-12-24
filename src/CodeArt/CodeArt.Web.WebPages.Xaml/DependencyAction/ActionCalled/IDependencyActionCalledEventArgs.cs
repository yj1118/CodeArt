using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    public interface IDependencyActionCalledEventArgs
    {
        /// <summary>
        /// 获取或设置返回值
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// 行为需要的参数列表
        /// </summary>
        object[] Arguments { get; }

        DependencyAction Action { get; }
    }
}
