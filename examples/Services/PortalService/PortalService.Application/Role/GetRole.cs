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
    [Service("getRole")]
    public class GetRole : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var detail = RoleCommon.FindBy(arg.Id, QueryLevel.None);
            return DTObject.CreateReusable("{id,name,markedCode,description,organization:{id},permissions:[{id,name}],isSystem}", detail);
        }
    }
}