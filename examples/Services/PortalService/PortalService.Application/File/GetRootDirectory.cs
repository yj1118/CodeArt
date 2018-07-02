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
    /// <summary>
    /// 获取磁盘根目录信息
    /// </summary>
    [SafeAccess]
    [Service("GetRootDirectory")]
    public class GetRootDirectory : ServiceProvider
    {
        protected override DTObject DynamicInvoke(dynamic arg)
        {
            VirtualDisk disk = VirtualDiskCommon.FindById(arg.DiskId, QueryLevel.None);
            return DTObject.CreateReusable("{id,name}", disk.Root);
        }
    }
}