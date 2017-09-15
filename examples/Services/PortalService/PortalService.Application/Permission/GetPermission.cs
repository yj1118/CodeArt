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
    [Service("getPermission")]
    public class GetPermission : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var detail = PermissionCommon.FindById(arg.Id, QueryLevel.None);
            return DTObject.CreateReusable("{id,name,markedCode,description}", detail);
        }
    }
}