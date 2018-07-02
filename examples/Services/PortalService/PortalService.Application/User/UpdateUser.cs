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
using UserSubsystem;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("updateUser")]
    public class UpdateUser : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new UserSubsystem.UpdateUser(arg.Id)
            {
                Name = arg.Name,
                PhotoId = arg.PhotoId ?? arg.Photo?.Id ?? Guid.Empty,
                Email = arg.Email,
                MobileNumber = arg.MobileNumber
            };

            if(arg.Sex != null)
            {
                cmd.Sex = (Sex)arg.GetValue<byte>("sex");
            }

            cmd.Execute();
            return DTObject.Empty;
        }
    }
}
