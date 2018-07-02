using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    public sealed class DependencyActionPreCallEventArgs : IDependencyActionPreCallEventArgs
    {
        /// <summary>
        /// 获取或设置是否允许执行行为
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        /// 行为需要的参数
        /// </summary>
        public object[] Arguments { get; set; }

        public DependencyAction Action { get; private set; }

        public object ReturnValue { get; set; }

        public DependencyActionPreCallEventArgs(DependencyAction action, object[] args)
        {
            this.Action = action;
            this.Arguments = args;
            this.Allow = true; //默认是允许执行的
            this.ReturnValue = null;
        }

    }
}
