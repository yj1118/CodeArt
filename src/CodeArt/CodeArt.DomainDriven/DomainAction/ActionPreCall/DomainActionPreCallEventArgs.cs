using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class DomainActionPreCallEventArgs : IDomainActionPreCallEventArgs
    {
        /// <summary>
        /// 获取或设置是否允许执行行为
        /// </summary>
        public bool Allow { get; set; }

        /// <summary>
        /// 行为需要的参数
        /// </summary>
        public object[] Arguments { get; set; }

        public DomainAction Action { get; private set; }

        public object ReturnValue { get; set; }

        public DomainActionPreCallEventArgs(DomainAction action, object[] arguments)
        {
            this.Action = action;
            this.Arguments = arguments;
            this.Allow = true; //默认是允许执行的
            this.ReturnValue = null;
        }

    }
}
