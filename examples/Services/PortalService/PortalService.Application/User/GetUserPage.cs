using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using UserSubsystem;
using CodeArt.DomainDriven;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("getUserPage")]
    public class GetUserPage : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var page = UserCommon.FindPage(arg.Name, arg.RoleMarkedCode,arg.PageIndex, arg.PageSize);
            return DTObjectPro.Create("{id,name,sex}", page);
        }
    }
}