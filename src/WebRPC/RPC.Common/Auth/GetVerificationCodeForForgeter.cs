using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;

namespace RPC.Common
{
    [Procedure("GetVerificationCodeForForgeter")]
    [SafeAccess()]
    public class GetVerificationCodeForForgeter : Procedure
    {
        public override DTObject Invoke(DTObject arg)
        {
            return ServiceContext.Invoke("getVerificationCodeForForgeter", arg);
        }
    }
}
