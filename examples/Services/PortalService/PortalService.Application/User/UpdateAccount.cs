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
    [Service("updateAccount")]
    public class UpdateAccount : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new AccountSubsystem.UpdateAccount(arg.Id)
            {
                Name = arg.Name,
                Email = arg.Email,
                IsEnabled = arg.IsEnabled,
                MobileNumber = arg.MobileNumber,
                Password = arg.Password,
                RoleIds = arg.RoleIds != null ? arg.RoleIds?.OfType<Guid>() : null
            };
            cmd.Execute();
            return DTObject.Empty;
        }
    }
}
