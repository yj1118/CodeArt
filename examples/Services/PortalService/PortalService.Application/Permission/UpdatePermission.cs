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
    [Service("updatePermission")]
    public class UpdatePermission : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new AccountSubsystem.UpdatePermission(arg.Id)
            {
                Name = arg.Name,
                Description = arg.Description,
                MarkedCode = arg.MarkedCode
            };
            var permission = cmd.Execute();
            return DTObject.CreateReusable("{id,name,description,markedCode}", permission);
        }
    }
}
