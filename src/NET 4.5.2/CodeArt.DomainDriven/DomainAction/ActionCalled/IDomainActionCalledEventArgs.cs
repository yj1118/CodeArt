using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDriven
{
    public interface IDomainActionCalledEventArgs
    {
        /// <summary>
        /// 获取或设置返回值
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// 行为需要的参数列表
        /// </summary>
        object[] Arguments { get; }

        DomainAction Action { get; }
    }
}
