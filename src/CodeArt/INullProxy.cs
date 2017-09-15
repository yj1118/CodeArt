using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt
{
    /// <summary>
    /// 表示对象使用空代理来表示自身为null
    /// </summary>
    public interface INullProxy
    {
        /// <summary>
        /// 对象是否为null
        /// </summary>
        /// <returns></returns>
        bool IsNull();
    }
}
