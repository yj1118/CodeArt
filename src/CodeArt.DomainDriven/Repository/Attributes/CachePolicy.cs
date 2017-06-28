using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 仓储的实现需要提供的缓存策略
    /// </summary>
    public class CachePolicy
    {
        /// <summary>
        /// 获取或设置一个值，该值指示是否应在给定时段内未访问，就会对它进行是否逐出缓存项。
        /// </summary>
        public TimeSpan SlidingExpiration { get; set; }

        /// <summary>
        /// 指示缓存项是否永久不过期
        /// </summary>
        public bool NotRemovable { get; set; }

        /// <summary>
        /// 指示项不被缓存
        /// </summary>
        public bool NoCache { get; set; }


        public CachePolicy()
        {
            this.SlidingExpiration = TimeSpan.FromMinutes(10); //默认缓存10分钟
            this.NotRemovable = false;
            this.NoCache = false;
        }
    }
}
