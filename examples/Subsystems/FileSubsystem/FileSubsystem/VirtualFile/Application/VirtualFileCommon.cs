using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace FileSubsystem
{
    public static class VirtualFileCommon
    {
        public static VirtualFile FindById(Guid id, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualFileRepository>();
            return repository.Find(id, level);
        }

        public static VirtualFile FindById(Guid id)
        {
            return VirtualFileCommon.FindById(id, QueryLevel.None);
        }

        public static VirtualFile FindByStoreKey(string storeKey, QueryLevel level)
        {
            var repository = Repository.Create<IVirtualFileRepository>();
            return repository.FindBy(storeKey, level);
        }

        /// <summary>
        /// 获取目录下的文件，可以指定获取多少个
        /// </summary>
        /// <param name="directoryId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static IEnumerable<VirtualFile> FindFilesByDirectory(Guid directoryId, int count)
        {
            var repository = Repository.Create<IVirtualFileRepository>();
            var page = repository.FindsByDirectory(directoryId, 0, count);  //用翻页接口模拟效果
            return page.Objects;
        }

        public static Page<VirtualFile> FindFilesByPage(Guid directoryId, int pageIndex, int pageSize)
        {
            var repository = Repository.Create<IVirtualFileRepository>();
            return repository.FindsByDirectory(directoryId, pageIndex, pageSize);
        }

        //public static  List<VirtualFile> LoadFiles(IList<string> storeKeys)
        //{
        //    List<VirtualFile> files = new List<VirtualFile>();
        //    foreach (var storeKey in storeKeys)
        //    {
        //        var file = VirtualFileCommon.FindByFilekey(storeKey, QueryLevel.Single);
        //        if (file.IsEmpty()) throw new DomainDrivenException("没有找到文件编号为 " + key + " 的文件");
        //        files.Add(file);
        //    }
        //    return files;
        //}
    }
}
