using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using FileSubsystem;

namespace FileSubsystemTest
{

    [TestClass]
    public class UsedSizeTest : DomainStage
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
        public void AddFileWithSimple()
        {
            var disk = Util.AddDisk("1", "1号磁盘");
            var directory = Util.AddCommonDirectory("directory", disk.Id, Guid.Empty);
            var storeKey1 = Guid.NewGuid().ToString();
            var storeKey2 = Guid.NewGuid().ToString();


            var file1 = Util.AddFile("test1", "txt", storeKey1, 100, disk.Id, directory.Id);
            Util.AssertUsedSize(100, disk.Id);

            
            var file2 = Util.AddFile("test2", "txt", storeKey2, 200, disk.Id, directory.Id);
            Util.AssertUsedSize(300, disk.Id);

        }
        /// <summary>
        /// 在不同文件目录中添加文件，和在同一目录中添加多个文件
        /// </summary>
        [TestMethod]
        public void AddFileWithComplicated()
        {
            var disk = Util.AddDisk("1", "1号磁盘", 10240);
            var directory1 = Util.AddSystemDirectory("directory 1", disk.Id);
            var directory2 = Util.AddCommonDirectory("directory 2", disk.Id, Guid.Empty);
            var directory3 = Util.AddCommonDirectory("directory 3", disk.Id, directory1.Id);
            var storeKey1 = Guid.NewGuid().ToString();
            var storeKey2 = Guid.NewGuid().ToString();
            var storeKey3 = Guid.NewGuid().ToString();
            var storeKey4 = Guid.NewGuid().ToString();
            var file1 = Util.AddFile("test1", "txt", storeKey1, 100, disk.Id, directory1.Id);
            Util.AssertUsedSize(100, disk.Id);


            var file2 = Util.AddFile("test2", "txt", storeKey2, 200, disk.Id, directory2.Id);
            Util.AssertUsedSize(300, disk.Id);


            var file3 = Util.AddFile("test3", "txt", storeKey3, 300, disk.Id, directory3.Id);
            Util.AssertUsedSize(600, disk.Id);

            var file4 = Util.AddFile("test3", "txt", storeKey4, 424, disk.Id, directory2.Id);
            Util.AssertUsedSize(1024, disk.Id);

        }

        [TestMethod]
        public void DeleteFileWithSimple()
        {
            var disk = Util.AddDisk("1", "1号磁盘", 10240);
            var directory = Util.AddSystemDirectory("directory 1", disk.Id);
            var storeKey1 = Guid.NewGuid().ToString();
            var storeKey2 = Guid.NewGuid().ToString();

            var file1 = Util.AddFile("test1", "txt", storeKey1, 100, disk.Id, directory.Id);
            var file2 = Util.AddFile("test2", "txt", storeKey2, 200, disk.Id, directory.Id);

            Util.DeleteFile(disk.Id, file1.Id);
            Util.AssertUsedSize(200, disk.Id);

            Util.DeleteFile(disk.Id, file2.Id);
            Util.AssertUsedSize(0, disk.Id);

        }
     
        [TestMethod]
        public void DeleteFileWithComplicated()
        {
            var disk = Util.AddDisk("1", "1号磁盘", 10240);
            var directory1 = Util.AddSystemDirectory("directory 1", disk.Id);
            var directory2 = Util.AddCommonDirectory("directory 2", disk.Id, Guid.Empty);
            var directory3 = Util.AddCommonDirectory("directory 3", disk.Id, directory1.Id);
            var storeKey1 = Guid.NewGuid().ToString();
            var storeKey2 = Guid.NewGuid().ToString();
            var storeKey3 = Guid.NewGuid().ToString();
            var storeKey4 = Guid.NewGuid().ToString();
            var file1 = Util.AddFile("test1", "txt", storeKey1, 100, disk.Id, directory1.Id);
            var file2 = Util.AddFile("test2", "txt", storeKey2, 200, disk.Id, directory2.Id);
            var file3 = Util.AddFile("test2", "txt", storeKey3, 300, disk.Id, directory3.Id);
            var file4 = Util.AddFile("test3", "txt", storeKey4, 424, disk.Id, directory2.Id);
            Util.DeleteFile(disk.Id, file1.Id);
            Util.AssertUsedSize(924, disk.Id);

            Util.DeleteFile(disk.Id, file2.Id);
            Util.AssertUsedSize(724, disk.Id);

            Util.DeleteFile(disk.Id, file3.Id);
            Util.AssertUsedSize(424, disk.Id);

            Util.DeleteFile(disk.Id, file4.Id);
            Util.AssertUsedSize(0, disk.Id);

        }
    }
}
    
