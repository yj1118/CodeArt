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
    [Service("AddFile")]
    public class AddFile : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            var cmd = new CreateVirtualFile(arg.Name, arg.Extension, arg.StoreKey, arg.Size, arg.DirectoryId);
            var file = cmd.Execute();
            return DTObject.CreateReusable("{id,name,storeKey,extension,size}", file);
        }
    }
}