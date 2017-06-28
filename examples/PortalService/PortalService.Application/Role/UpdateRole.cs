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
    [Service("updateRole")]
    public class UpdateRole : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new AccountSubsystem.UpdateRole(arg.Id)
            {
                Name = arg.Name,
                Description = arg.Description,
                MarkedCode = arg.MarkedCode,
                PermissionIds = arg.PermissionIds
            };
            var role = cmd.Execute();
            return DTObject.Empty;
        }
    }
}
