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
    [Service("getPermissionPage")]
    public class GetPermissionPage : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var page = PermissionCommon.FindPage(arg.Name, arg.PageIndex, arg.PageSize);
            return DTObjectPro.Create("{id,name}", page);
        }
    }
}