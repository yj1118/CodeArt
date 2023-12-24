using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    public sealed class DependencyActionCalledEventArgs : IDependencyActionCalledEventArgs
    {
        /// <summary>
        /// 行为需要的参数
        /// </summary>
        public object[] Arguments { get; set; }

        public DependencyAction Action { get; private set; }

        public object ReturnValue { get; set; }

        public DependencyActionCalledEventArgs(DependencyAction action, object[] args, object returnValue)
        {
            this.Action = action;
            this.Arguments = args;
            this.ReturnValue = returnValue;
        }
    }
}