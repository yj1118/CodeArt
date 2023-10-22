using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 对象不能null的接口
    /// </summary>
    public interface INotNullObject : INullProxy
    {
        /// <summary>
        /// 对象是否为空
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}
