using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    /// <summary>
    /// 删除文件
    /// </summary>
    public sealed class DeleteVirtualFile : Command
    {
        private Guid _diskId;
        private Guid _fileId;

        public DeleteVirtualFile(Guid diskId, Guid fileId)
        {
            _diskId = diskId;
            _fileId = fileId;
        }

        protected override void ExecuteProcedure()
        {
            var disk = VirtualDiskCommon.LoadByMirroring(_diskId);
            var file = VirtualFileCommon.FindById(_fileId, QueryLevel.Mirroring);
            //文件删除后减少虚拟磁盘的大小
            disk.Delete(file);
            VirtualDiskCommon.Update(disk);
        }
    }
}
