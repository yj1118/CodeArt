using System;
using System.Linq;

using CodeArt;
using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public sealed class DeleteVirtualDirectory : Command
    {
        private Guid _diskId;
        private Guid _dirId;
        private bool _force;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diskId"></param>
        /// <param name="dirId"></param>
        /// <param name="force">是否强制删除系统目录</param>
        private DeleteVirtualDirectory(Guid diskId, Guid dirId, bool force)
        {
            _diskId = diskId;
            _dirId = dirId;
            _force = force;
        }

        /// <summary>
        /// 获取删除普通目录的事务
        /// </summary>
        /// <param name="diskId"></param>
        /// <param name="dirId"></param>
        /// <returns></returns>
        public static DeleteVirtualDirectory GetCommon(Guid diskId, Guid dirId)
        {
            return new DeleteVirtualDirectory(diskId, dirId, false);
        }

        /// <summary>
        /// 获取删除系统目录的事务
        /// </summary>
        /// <param name="diskId"></param>
        /// <param name="dirId"></param>
        /// <returns></returns>
        public static DeleteVirtualDirectory GetSystem(Guid diskId, Guid dirId)
        {
            return new DeleteVirtualDirectory(diskId, dirId, true);
        }


        protected override void ExecuteProcedure()
        {
            var disk = VirtualDiskCommon.LoadByMirroring(_diskId);
            VirtualDirectory dir = VirtualDirectoryCommon.FindById(_dirId, QueryLevel.Mirroring);

            //这里需要测试，dir内部的this.Disk和 disk要是同一个对象，在当前会话里都是同一个镜像
            //if(ReferenceEquals(disk, dir.Disk))
            //{

            //}

            if (dir.IsSystem && !_force) throw new BusinessException(Strings.CanNotDeleteSystemDirectory);
            disk.Delete(dir);
            VirtualDiskCommon.Update(disk);
        }
    }
}
