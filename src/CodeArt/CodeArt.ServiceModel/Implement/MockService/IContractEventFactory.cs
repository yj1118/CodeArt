using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;

namespace CodeArt.ServiceModel.Mock
{
    /// <summary>
    /// 契约事件工厂
    /// </summary>
    public interface IContractEventFactory
    {
        /// <summary>
        /// 根据dto定义，创建契约事件
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        IContractEvent Create(DTObject dto);
    }
}
