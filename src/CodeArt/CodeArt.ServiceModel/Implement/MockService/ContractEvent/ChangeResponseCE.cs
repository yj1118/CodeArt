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
    public class ChangeResponseCE : IContractEvent
    {
        private string _contractId;

        private ServiceResponse _response;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractId">需要更改的契约编号</param>
        /// <param name="response">更改后的响应值</param>
        public ChangeResponseCE(string contractId, ServiceResponse response)
        {
            _contractId = contractId;
            _response = response;
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        public void RaiseEvent(MockContract sender)
        {
            var package = sender.Package;
            var target = package?.Find(_contractId);
            if (target != null) target.Response = _response;
        }
    }
}
