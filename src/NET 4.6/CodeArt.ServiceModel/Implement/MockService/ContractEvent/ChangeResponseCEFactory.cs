using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.DTO;


namespace CodeArt.ServiceModel.Mock
{
    /// <summary>
    /// 契约事件,契约执行后可以引发一系列的事件
    /// </summary>
    [SafeAccess]
    public class ChangeResponseCEFactory : ContractEventFactory
    {
        protected override object[] GetArgs(DTObject args)
        {
            var contractId = args.GetValue<string>("contractId", string.Empty);
            var responseDTO = args.GetObject("response", DTObject.Empty);
            var response = ServiceResponse.Create(responseDTO);
            return new object[] { contractId, response };
        }
    }
}
