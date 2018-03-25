using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Net.Anycast
{
    public sealed class ReconnectArgs
    {
        /// <summary>
        /// 已开始重连的次数
        /// </summary>
        public int Times
        {
            get;
            internal set;
        }

        public ReconnectArgs()
        {
            this.Times = 0;
        }

        public ReconnectArgs Clone()
        {
            return new ReconnectArgs() { Times = this.Times };
        }

    }
}
