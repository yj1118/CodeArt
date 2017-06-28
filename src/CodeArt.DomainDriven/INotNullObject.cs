using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDriven
{
    /// <summary>
    /// 不能为null对象的接口
    /// </summary>
    public interface INotNullObject
    {
        /// <summary>
        /// 对象是否为空
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
    }
}
