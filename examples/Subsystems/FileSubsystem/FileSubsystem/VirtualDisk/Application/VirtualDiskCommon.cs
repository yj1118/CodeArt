using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public static class VirtualDiskCommon
    {
        public static VirtualDisk FindById(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualDiskRepository>();
            return repository.Find(id, level);
        }

        public static VirtualDisk FindByMarkedCode(string markedCode, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualDiskRepository>();
            return repository.FindBy(markedCode, level);
        }

        public static Page<VirtualDisk> FindPage(string markedCode, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IVirtualDiskRepository>();
            return repository.FindsBy(markedCode, pageIndex, pageSize);
        }


        #region 工具方法

        internal static VirtualDisk LoadByMirroring(Guid diskId)
        {
            var disk = FindById(diskId, QueryLevel.Mirroring);
            if (disk.IsEmpty()) throw new BusinessException(string.Format(Strings.NotFoundVirtualDisk, diskId));
            return disk;
        }


        internal static void Update(VirtualDisk disk)
        {
            var repository = Repository.Create<IVirtualDiskRepository>();
            repository.Update(disk);
        }

        #endregion


    }
}
