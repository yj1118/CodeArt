using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DTO
{
    /// <summary>
    /// 由于动态类型不识别扩展方法，因此使用该包装集合
    /// </summary>
    public sealed class PrimitiveValueList : List<object>
    {
        public PrimitiveValueList()
        {
        }

        public PrimitiveValueList(IEnumerable<object> items)
            : base(items)
        {
        }

        public IEnumerable<T> OfType<T>()
        {
            return Enumerable.OfType<T>(this);
        }
    }
}
