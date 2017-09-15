using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 缓冲条目
    /// </summary>
    public class BufferEntry
    {
        /// <summary>
        /// 缓存的聚合根
        /// </summary>
        public IAggregateRoot Root
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据版本号
        /// </summary>
        public int DataVersion
        {
            get
            {
                return this.Root.DataVersion;
            }
        }


        public BufferEntry(IAggregateRoot root)
        {
            this.Root = root;
        }
    }
}
