using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using FileSubsystem;

namespace FileSubsystemTest
{
    [TestClass]
    public class DiskTest : DomainStage
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
            var disk = Util.AddDisk("tiny", "微小的磁盘空间");
            this.Fixture.Add(disk);
        }

        protected override void PreLeaveScene()
        {
            var disk = this.Fixture.Get<VirtualDisk>();
            Util.DeleteDisk(disk.Id);
        }


        [TestMethod]
        public void Add()
        {
            var disk = VirtualDiskCommon.FindByMarkedCode("tiny", QueryLevel.None);
            Assert.AreEqual("tiny", disk.MarkedCode);
            Assert.AreEqual("微小的磁盘空间", disk.Description);
        }



        [TestMethod]
        public void Update()
        {
            var id = this.Fixture.Get<VirtualDisk>().Id;

            Util.UpdateDisk(id, "tiny1", "编辑描述");

            var disk = VirtualDiskCommon.FindByMarkedCode("tiny1", QueryLevel.None);
            Assert.AreEqual("编辑描述", disk.Description);
            Assert.AreEqual("tiny1", disk.MarkedCode);
        }


        [TestMethod]
        public void Delete()
        {
            var diskId = Guid.Empty;

            {
                var disk = Util.AddDisk(null, null);
                diskId = disk.Id;
            }

            Util.DeleteDisk(diskId);

            {
                var obj = VirtualDiskCommon.FindById(diskId, QueryLevel.None);
                Assert.IsTrue(obj.IsEmpty());
            }
        }

    }
}
