using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public sealed class CommandFailedEventArgs
    {
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// 指示是否抛出异常
        /// </summary>
        public bool ThrowError { get; set; }

        public CommandFailedEventArgs(Exception exception)
        {
            this.Exception = exception;
            this.ThrowError = true;
        }

    }
}
