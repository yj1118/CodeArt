using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public static class VirtualDirectoryCommon
    {
        public static VirtualDirectory FindById(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.Find(id, level);
        }

        public static VirtualDirectory FindRoot(Guid diskId, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.FindRoot(diskId, level);
        }


        internal static IEnumerable<VirtualDirectory> FindChilds(Guid parentId, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.FindChilds(parentId, level);
        }

        public static IEnumerable<VirtualDirectory> FindChilds(Guid parentId)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.FindChilds(parentId, QueryLevel.None);
        }

        /// <summary>
        /// 获取直系子目录的个数
        /// </summary>
        /// <param name="diskId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static int GetChildCount(Guid parentId)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.GetChildCount(parentId, QueryLevel.None);
        }


        public static Page<VirtualDirectory> FindChildsByPage(Guid parentId, int pageIndex,int pageSize)
        {
            var repository = Repository.Create<IVirtualDirectoryRepository>();
            return repository.FindPageByParent(parentId, pageIndex, pageSize);
        }
    }
}
