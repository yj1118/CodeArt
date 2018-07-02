using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;

using UserSubsystem;
using FileSubsystem;
using AccountSubsystem;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("addUser")]
    public class AddUser : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            Guid[] roleIds = Array.Empty<Guid>();
            if(arg.RoleMarkedCode != null)
            {
                string markedCode = arg.RoleMarkedCode.ToString();
                var role = RoleCommon.FindByMarkedCode(markedCode, QueryLevel.None);
                if(!role.IsEmpty())
                {
                    roleIds = new Guid[] { role.Id };
                }
            }
            else
            {
                roleIds = arg.RoleIds != null ? arg.RoleIds?.OfType<Guid>() : Array.Empty<Guid>();
            }

            var cmd = new CreateUser(arg.AccountName, arg.Password, arg.DiskSize ?? VirtualDisk.Size1G) //默认给予1G的大小
            {
                Email = arg.Email,
                LocationId = arg.LocationId,
                MobileNumber = arg.MobileNumber,
                Name = arg.Name,
                NickName = arg.NickName,
                PhotoId = arg.PhotoId,
                RoleIds = arg.RoleIds != null ? arg.RoleIds?.OfType<Guid>() : Array.Empty<Guid>(),
                Sex = arg.Sex,
                IsEnabled = arg.IsEnabled
            };

            var user = cmd.Execute();
            return DTObject.CreateReusable("{id}", user);
        }
    }
}