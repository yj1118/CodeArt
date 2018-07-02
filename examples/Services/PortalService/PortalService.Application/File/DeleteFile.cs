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
    [Service("DeleteFile")]
    public class DeleteFile : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            if(arg.StoreKey != null)
            {
                VirtualFile file = VirtualFileCommon.FindByStoreKey(arg.StoreKey, QueryLevel.None);
                if (!file.IsEmpty())
                {
                    var cmd = new DeleteVirtualFile(file.Disk.Id, file.Id);
                    cmd.Execute();
                }
            }
            else
            {
                VirtualFile file = VirtualFileCommon.FindById(arg.Id, QueryLevel.None);
                if (!file.IsEmpty())
                {
                    var cmd = new DeleteVirtualFile(file.Disk.Id, file.Id);
                    cmd.Execute();
                }
            }
            return DTObject.Empty;
        }
    }
}