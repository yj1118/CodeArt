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
    [Service("updatePassword")]
    public class UpdatePassword : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new AccountSubsystem.UpdatePassword(arg.Id ?? arg.AcconutId, arg.Name ?? arg.AccountName, arg.OldPassword, arg.NewPassword);
            cmd.Execute();
            return DTObject.Empty;
        }
    }
}
