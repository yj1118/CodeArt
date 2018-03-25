using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using AccountSubsystem;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("addRole")]
    public class AddRole : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new CreateRole(arg.Name, arg.PermissionIds?.OfType<Guid>(), arg.IsSystem ?? false)
            {
                Description = arg.Description,
                MarkedCode = arg.MarkedCode,
                OrganizationId = arg.OrganizationId ?? Guid.Empty
            };

            var role = cmd.Execute();
            return DTObject.CreateReusable("{id}", role);
        }
    }
}