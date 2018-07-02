using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using FileSubsystem;
using Dapper;

namespace FileSubsystemTest
{
    [TestClass]
    public class FileTest : DomainStage
    {
        protected override void PreEnterScene()
        {
            //进入场景之前
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        protected override void EnteredScene()
        {

        }

        protected override void PreLeaveScene()
        {

        }

        
       
        [TestMethod]
        public void Add()
        {
            var disk = Util.AddDisk("1", "1号磁盘", 10240);
            var common1 = Util.AddCommonDirectory("common 1", disk.Id, Guid.Empty);
            var storeKey1 = Guid.NewGuid().ToString();
            var storeKey2 = Guid.NewGuid().ToString();
            var storeKey3 = Guid.NewGuid().ToString();

            var file1 = Util.AddFile("test1", "txt", storeKey1, 100, disk.Id, common1.Id);
            var file2 = Util.AddFile("test2", "txt", storeKey2, 200, disk.Id, common1.Id);
            var file3 = Util.AddFile("test3", "txt", storeKey3, 300, disk.Id, common1.Id);


            Util.AssertVirtualFileByDB(file1.Id, ("test1", "txt", storeKey1, 100, disk.Id, common1.Id));
            Util.AssertVirtualFileByDB(file2.Id, ("test2", "txt", storeKey2, 200, disk.Id, common1.Id));
            Util.AssertVirtualFileByDB(file3.Id, ("test3", "txt", storeKey3, 300, disk.Id, common1.Id));

            Util.AssertVirtualFile(file1, "test1", "txt", storeKey1, 100, disk.Id, common1.Id);
            Util.AssertVirtualFile(file2, "test2", "txt", storeKey2, 200, disk.Id, common1.Id);
            Util.AssertVirtualFile(file3, "test3", "txt", storeKey3, 300, disk.Id, common1.Id);
        }
       
       
        


        [TestMethod]
        public void Update()
        {
            var disk = Util.AddDisk("1", "1号磁盘");
            var common1 = Util.AddCommonDirectory("common 1", disk.Id, Guid.Empty);
            var common2 = Util.AddCommonDirectory("common 2", disk.Id, Guid.Empty);
            var storeKey = Guid.NewGuid().ToString();
            var file = Util.AddFile("test", "txt", storeKey, 1024, disk.Id, common1.Id);

            var cmd = new UpdateVirtualFile(file.Id)
            {
                Name = "update",
                DirectoryId = common2.Id,
                Size = 2048/*只可以改变文件名和存放路径，无法改变大小,改变大小无效*/
            };
            var updateFile = cmd.Execute();

            Util.AssertVirtualFileByDB(file.Id, ("update", "txt", storeKey, 2048, disk.Id, common2.Id));
            Util.AssertVirtualFile(updateFile, "update", "txt", storeKey, 2048, disk.Id, common2.Id);
        }


        [TestMethod]
        public void Delete()
        {
            var disk = Util.AddDisk("1", "1号磁盘");
            var common1 = Util.AddCommonDirectory("common 1", disk.Id, Guid.Empty);
            var storeKey = Guid.NewGuid().ToString();
            var file = Util.AddFile("test", "txt", storeKey, 1024, disk.Id, common1.Id);
            Util.DeleteFile(disk.Id, file.Id);

            Util.AssertExistFileByDB(file.Id);
            Util.AssertExistFile(file.Id);

            
        }
       
    }
}
