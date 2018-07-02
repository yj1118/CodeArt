using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DTO;
using CodeArt.ServiceModel;
using CodeArt.Concurrent;

using AccountSubsystem;
using FileSubsystem;
using CodeArt.DomainDriven;

namespace PortalService.Application
{
    [SafeAccess]
    [Service("GetFile")]
    public class GetFile : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var file = VirtualFileCommon.FindById(arg.Id, QueryLevel.None);
            return DTObject.CreateReusable("{id,name,storeKey}", file);
        }
    }
}