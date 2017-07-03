using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.ServiceModel.Mock
{
    /// <summary>
    /// 契约事件,契约执行后可以引发一系列的事件
    /// </summary>
    public interface IContractEvent
    {
        /// <summary>
        /// 触发事件
        /// </summary>
        void RaiseEvent(MockContract sender);
    }
}
