using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using FileSubsystem;
using Dapper;

namespace FileSubsystemTest
{
    public static class Util
    {
        public static VirtualDisk AddDisk(string markedCode, string description, long size = 1024)
        {
            var cmd = new CreateVirtualDisk(Guid.NewGuid(), 1024)
            {
                MarkedCode = markedCode,
                Description = description
            };
            return cmd.Execute();
        }

        public static void DeleteDisk(Guid id)
        {
            var cmd = new DeleteVirtualDisk(id);
            cmd.Execute();
        }

        public static void UpdateDisk(Guid id, string markedCode, string description)
        {
            var cmd = new UpdateVirtualDisk(id)
            {
                Description = description,
                MarkedCode = markedCode
            };
            cmd.Execute();
        }

        /// <summary>
        /// 添加一个普通目录
        /// </summary>
        /// <param name="name">目录名称</param>
        /// <param name="diskId">所属磁盘编号</param>
        /// <param name="parentId">父编号,可以为空，如果为空，就是磁盘的根目录下的子目录</param>
        /// <returns></returns>
        public static VirtualDirectory AddCommonDirectory(string name, Guid diskId, Guid parentId)
        {
            var cmd = CreateVirtualDirectory.GetCommon(name, diskId, parentId);
            return cmd.Execute();
        }

        public static VirtualDirectory AddSystemDirectory(string name, Guid diskId)
        {
            var cmd = CreateVirtualDirectory.GetSystem(name, diskId);
            return cmd.Execute();
        }
        public static VirtualFile AddFile(string name, string extension, string storeKey, long size, Guid diskId, Guid directoryId)
        {
            var cmd = new CreateVirtualFile(name, extension, storeKey, size, diskId, directoryId);
            return cmd.Execute();
        }
      
        public static void DeleteFile(Guid diskId, Guid fileid)
        {
            var cmd = new DeleteVirtualFile(diskId, fileid);
            cmd.Execute();
        }
        /// <summary>
        /// 在数据库断言
        /// </summary>
        /// <param name="id"></param>
        /// <param name="diskId"></param>
        /// <param name="expected"></param>
        public static void AssertVirtualFileByDB(Guid id, (string name, string extension, string storeKey, long size,Guid diskId, Guid directoryId) expected)
        {
            DataPortal.Direct<VirtualFile>((conn) =>
            {
                {
                    var file = conn.QuerySingle("select dbo.VirtualFile.*, dbo.VirtualDirectory.DiskId from dbo.VirtualFile inner join dbo.VirtualDirectory on dbo.VirtualDirectory.Id=dbo.VirtualFile.DirectoryId where dbo.VirtualFile.Id=@id", new { id });
                    Assert.AreEqual(expected.name, file.Name);
                    Assert.AreEqual(expected.extension, file.Extension);
                    Assert.AreEqual(expected.storeKey, file.StoreKey);
                    Assert.AreEqual(expected.size, file.Size);
                    Assert.AreEqual(expected.directoryId, file.DirectoryId);
                    Assert.AreEqual(expected.diskId, file.DiskId);
                }
            });
        }
        /// <summary>
        /// 在内存上断言
        /// </summary>
        /// <param name="id"></param>
        /// <param name="diskId"></param>
        /// <param name="expected"></param>
        public static void AssertVirtualFile(VirtualFile file,string name, string extension, string storeKey, long size, Guid diskId, Guid directoryId)
        {
            Assert.AreEqual(name, file.Name);
            Assert.AreEqual(extension, file.Extension);
            Assert.AreEqual(storeKey, file.StoreKey);
            Assert.AreEqual(size, file.Size);
            Assert.AreEqual(diskId, file.Disk.Id);
            Assert.AreEqual(directoryId, file.Directory.Id);
        }
        public static void AssertExistFileByDB(Guid id)
        {
            DataPortal.Direct<VirtualFile>((conn) =>
            {
                {
                    var count = conn.QuerySingle("select count(*) as number from dbo.VirtualFile where id=@id", new { id });
                    Assert.AreEqual(0, count.number);
                }
            });
        }
        public static void AssertExistFile(Guid id)
        {
            var obj = VirtualFileCommon.FindById(id);
            Assert.IsTrue(obj.IsEmpty());
        }
     
        public static void AssertUsedSize(long size,Guid id)
        {
            long usedSize = 0;
            DataPortal.Direct<VirtualFile>((conn) =>
            {
                var result = conn.QuerySingle("select * from dbo.VirtualDisk where id=@id", new { id });
                usedSize = result.UsedSize;
            });
            Assert.AreEqual(size, usedSize);
        }
    }
      
}
