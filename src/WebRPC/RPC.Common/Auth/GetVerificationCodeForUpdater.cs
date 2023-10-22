using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.Web.RPC;
using CodeArt.Concurrent;
using CodeArt.ServiceModel;
using CodeArt.AppSetting;

namespace RPC.Common
{
    [Procedure("GetVerificationCodeForUpdater")]
    [SafeAccess()]
    public class GetVerificationCodeForUpdater : Procedure
    {
        public override DTObject Invoke(DTObject arg)
        {
            arg["UserId"] = AppSession.PrincipalId;
            return ServiceContext.Invoke("getVerificationCodeForUpdater", arg);
        }
    }
}
