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
    [Service("getRolePage")]
    public class GetRolePage : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var page = RoleCommon.FindPage(arg.OrganizationId ?? Guid.Empty, arg.Name, arg.PageIndex, arg.PageSize);
            return DTObjectPro.Create("{id,name,markedCode,description,isSystem,permissions:[{id,name}]}", page);
        }
    }
}