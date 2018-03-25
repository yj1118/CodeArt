using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using UserSubsystem;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("initSA")]
    public class InitSA : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new UserSubsystem.InitSA(arg.Name,arg.Password)
            {
                Email = arg.Email
            };

            var user = cmd.Execute();
            return DTObject.CreateReusable("{id}", user);
        }
    }
}