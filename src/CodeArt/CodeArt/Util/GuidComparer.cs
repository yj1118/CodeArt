using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Util
{
    /// <summary>
    /// 提供Guid格式的字符串的比较
    /// </summary>
    public class GuidComparer : IEqualityComparer<string>
    {
        private bool _cache;

        private GuidComparer(bool cache) { _cache = cache; }

        private static Func<string, Guid> _guid = LazyIndexer.Init<string, Guid>((value)=>
        {
            return Guid.Parse(value);
        });

        public bool Equals(string x, string y)
        {
            if(_cache)
            {
                return _guid(x) == _guid(y);
            }
            else
            {
                if (Guid.TryParse(x, out var gX))
                {
                    if (Guid.TryParse(y, out var gY))
                    {
                        return gX == gY;
                    }
                }
            }

            return false;
        }

        public int GetHashCode(string obj)
        {
            return _guid(obj).GetHashCode();
        }

        /// <summary>
        /// 在对比的时候，会将字符串对应的Guid缓存，提高性能,适合需要对比的字符串个数是有限的情况
        /// </summary>

        public static readonly GuidComparer Cache = new GuidComparer(true);


        /// <summary>
        /// 每次对比时，都会将字符串转换为Guid,适合需要对比的字符串个数是无限大的情况
        /// </summary>
        public static readonly GuidComparer NoCache = new GuidComparer(false);

    }
}
