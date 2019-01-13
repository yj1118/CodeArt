using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DTO;
using CodeArt.ModuleNest;
using CodeArt.ServiceModel;

namespace Module.WebUI
{
    [SafeAccess()]
    [ModuleHandler("getVerificationCode")]
    public class GetVerificationCode : ModuleHandlerBase
    {
        public override DTObject Process(DTObject arg)
        {
            ServiceContext.Invoke("GetVerificationCode", arg);
            return DTObject.Empty;
        }
    }
}
