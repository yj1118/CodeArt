using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public class CacheEntry
    {
        /// <summary>
        /// 缓存的对象
        /// </summary>
        public object Object
        {
            get;
            private set;
        }

        /// <summary>
        /// 缓存该对象时使用的查询级别
        /// </summary>
        public int DataVersion
        {
            get;
            private set;
        }


        public CacheEntry(object obj, int dataVersion)
        {
            this.Object = obj;
            this.DataVersion = dataVersion;
        }
    }
}
