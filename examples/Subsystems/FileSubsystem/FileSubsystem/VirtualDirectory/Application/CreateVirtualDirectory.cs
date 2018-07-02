using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class CreateVirtualDirectory : Command<VirtualDirectory>
    {
        private string _name;
        private Guid _parentId;
        private Guid _diskId;
        private bool _isSystem;

        /// <summary>
        /// 获取创建普通目录的命令
        /// </summary>
        /// <param name="name"></param>
        /// <param name="diskId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static CreateVirtualDirectory GetCommon(string name, Guid diskId, Guid parentId)
        {
            return new CreateVirtualDirectory(name, diskId, parentId, false);
        }

        /// <summary>
        /// 获取创建系统目录的事务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="diskId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static CreateVirtualDirectory GetSystem(string name, Guid diskId)
        {
            return new CreateVirtualDirectory(name, diskId, Guid.Empty, true);
        }

        private CreateVirtualDirectory(string name, Guid diskId, Guid parentId, bool isSystem)
        {
            _name = name;
            _parentId = parentId;
            _diskId = diskId;
            _isSystem = isSystem;
        }

        protected override VirtualDirectory ExecuteProcedure()
        {
            var disk = LoadDisk();
            var directory = BuildDirectory(disk);
            disk.Create(directory);
            VirtualDiskCommon.Update(disk);
            return directory;
        }

        private VirtualDirectory BuildDirectory(VirtualDisk disk)
        {
            var dir = new VirtualDirectory(Guid.NewGuid(), disk);
            dir.Name = _name;
            dir.CreateTime = DateTime.Now;
            dir.IsSystem = _isSystem;
            SetParent(dir);
            return dir;
        }

        private VirtualDisk LoadDisk()
        {
            VirtualDisk disk = VirtualDiskCommon.FindById(_diskId, QueryLevel.Mirroring);
            if(disk.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDisk,_diskId));
            return disk;
        }

        private void SetParent(VirtualDirectory dir)
        {
            if (_parentId != Guid.Empty)
            {
                var parent = VirtualDirectoryCommon.FindById(_parentId, QueryLevel.Mirroring);
                if (!parent.IsEmpty()) dir.Parent = parent;
            }
        }

    }
}
