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
using AccountSubsystem;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("login")]
    public class Login : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new AccountSubsystem.Login(arg.Flag, arg.Password, arg.Ip);
            var account = cmd.Execute();
            if (account.IsEmpty()) return Failed;
            var user = UserCommon.FindById(account.Id, QueryLevel.None);

            var result = DTObject.CreateReusable("{id,displayName,photo:{storeKey},account:{email,roles:[{id,markedCode}]}}", user);
            result.Transform("email=account.email;roles=account.roles;!account");
            return result;
        }

        private static readonly DTObject Failed = DTObject.Create("{success:false}");

    }
}