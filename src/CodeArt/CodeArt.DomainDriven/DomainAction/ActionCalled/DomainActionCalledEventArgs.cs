using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class DomainActionCalledEventArgs : IDomainActionCalledEventArgs
    {
        /// <summary>
        /// 行为需要的参数
        /// </summary>
        public object[] Arguments { get; set; }

        public DomainAction Action { get; private set; }

        public object ReturnValue { get; set; }

        public DomainActionCalledEventArgs(DomainAction action, object[] arguments, object returnValue)
        {
            this.Action = action;
            this.Arguments = arguments;
            this.ReturnValue = returnValue;
        }
    }
}