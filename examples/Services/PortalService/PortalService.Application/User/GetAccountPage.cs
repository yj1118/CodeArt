using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using AccountSubsystem;
using CodeArt.DomainDriven;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("getAccountPage")]
    public class GetAccountPage : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var page = AccountCommon.FindPage(arg.Flag, arg.PageIndex, arg.PageSize);
            return DTObjectPro.Create("{id,name,email,status:{loginInfo:{lastTime,lastIp,total},isEnabled}}", page);
        }
    }
}