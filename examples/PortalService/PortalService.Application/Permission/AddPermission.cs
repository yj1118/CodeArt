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
    [Service("addPermission")]
    public class AddPermission : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new CreatePermission(arg.Name)
            {
                Description = arg.Description,
                MarkedCode = arg.MarkedCode
            };

            var permisson = cmd.Execute();
            return DTObject.CreateReusable("{id}", permisson);
        }
    }
}