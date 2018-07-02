using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using UserSubsystem;
using CodeArt.DomainDriven;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("getUser")]
    public class GetUser : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var user = UserCommon.FindById(arg.Id, QueryLevel.None);
            return DTObject.Create("{id,name,sex,photo:{id,name,createTime,size,storeKey,extension},account:{id,name,email,mobileNumber}}", user);
        }
    }
}