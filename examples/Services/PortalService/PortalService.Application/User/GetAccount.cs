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
    [Service("getAccount")]
    public class GetAccount : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var account = AccountCommon.FindById(arg.Id, QueryLevel.None);
            return DTObject.Create("{id,name,email,password,status:{isEnabled},roles:[{id,name}]}", account);
        }
    }
}