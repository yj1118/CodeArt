using System;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dapper;
using FileSubsystem;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;


namespace FileSubsystemTest
{
    [TestClass]
    public class DirectoryTest : DomainStage
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
            var disk = Util.AddDisk("1", "1号磁盘");
            var common1 = Util.AddCommonDirectory("common 1", disk.Id, Guid.Empty); //普通目录1
            var common1_1 = Util.AddCommonDirectory("common 1-1", disk.Id, common1.Id); //普通目录1-1
            var common1_1_1 = Util.AddCommonDirectory("common 1-1-1", disk.Id, common1_1.Id); //普通目录1-1-1
            var common1_1_2 = Util.AddCommonDirectory("common 1-1-2", disk.Id, common1_1.Id); //普通目录1-1-2

            var common1_2 = Util.AddCommonDirectory("common 1-2", disk.Id, common1.Id); //普通目录1-2
            var common1_3 = Util.AddCommonDirectory("common 1-3", disk.Id, common1.Id); //普通目录1-3

            var common2 = Util.AddCommonDirectory("common 2", disk.Id, Guid.Empty); //普通目录2
            var common2_1 = Util.AddCommonDirectory("common 2-1", disk.Id, common2.Id); //普通目录2-1
            var common2_2 = Util.AddCommonDirectory("common 2-2", disk.Id, common2.Id); //普通目录2-2
            var common2_3 = Util.AddCommonDirectory("common 2-3", disk.Id, common2.Id); //普通目录2-3

            var common3 = Util.AddCommonDirectory("common 3", disk.Id, Guid.Empty); //普通目录3

            this.Fixture.Add(disk);
            AddToFixture(common1, common1_1, common1_1_1, common1_1_2, common1_2, common1_3);
            AddToFixture(common2, common2_1, common2_2, common2_3);
            AddToFixture(common3);
        }

        private void AddToFixture(params VirtualDirectory[] directories)
        {
            foreach(var directory in directories)
            {
                this.Fixture.Add(directory.Name, directory);
            }
        }


        protected override void PreLeaveScene()
        {
            var disk = this.Fixture.Get<VirtualDisk>();
            Util.DeleteDisk(disk.Id);
        }


        [TestMethod]
        public void Add()
        {
            var common1 = this.Fixture.Get<VirtualDirectory>("common 1");
            AssertDirectory(common1.Id, common1.Name);

            var common2 = this.Fixture.Get<VirtualDirectory>("common 2");
            AssertDirectory(common2.Id, common2.Name);

            var common3 = this.Fixture.Get<VirtualDirectory>("common 3");
            AssertDirectory(common3.Id, common3.Name);
        }

        private void AssertDirectory(Guid id,string name)
        {
            DataPortal.Direct<VirtualDirectory>((conn) =>
            {
                var result = conn.QuerySingle("select name from dbo.virtualDirectory where id=@id", new { id });
                Assert.AreEqual(name, result.name);
            });

            var directory = VirtualDirectoryCommon.FindById(id, QueryLevel.Mirroring);

            //验证LR值
            var lr = GetLR(id);

            var childs = directory.Childs;
            if (childs.Count() == 0) return;
            if(childs.Count() == 1)
            {
                //子项只有一个节点时
                var child = childs.First();
                var childLR = GetLR(child.Id);
                Assert.AreEqual(lr.Left + 1, childLR.Left);
                Assert.AreEqual(lr.Right - 1, childLR.Right);

                AssertDirectory(child.Id, child.Name);
            }
            else
            {
                //子项有多个节点时，父节点的左值+1等于第一个子节点的左值，父节点的右值-1等于最后一个节点的右值
                var first = childs.First();
                var last = childs.Last();
                var firstLR = GetLR(first.Id);
                var lastLR = GetLR(last.Id);
                Assert.AreEqual(lr.Left + 1, firstLR.Left);
                Assert.AreEqual(lr.Right - 1, lastLR.Right);

                foreach(var child in childs)
                {
                    AssertDirectory(child.Id, child.Name);
                }
            }
            
        }

        private (int Left,int Right) GetLR(Guid id)
        {
            dynamic result = null;
            DataPortal.Direct<VirtualDirectory>((conn) =>
            {
                result = conn.QuerySingle("select lft,rgt from dbo.virtualDirectory where id=@id", new { id });
            });
            return (result.lft, result.rgt);
        }


        [TestMethod]
        public void Update()
        {
            var common1 = this.Fixture.Get<VirtualDirectory>("common 1");
            UpdateVirtualDirectory cmd = new UpdateVirtualDirectory(common1.Id)
            {
                Name = "common 1 改"
            };
            cmd.Execute();
            AssertDirectory(common1.Id, "common 1 改");
        }

        [TestMethod]
        public void Delete()
        {
            var disk = this.Fixture.Get<VirtualDisk>();
            var directory = this.Fixture.Get<VirtualDirectory>("common 1-1-1");
            DeleteVirtualDirectory cmd = DeleteVirtualDirectory.GetCommon(disk.Id, directory.Id);
            cmd.Execute();
            var obj = VirtualDirectoryCommon.FindById(directory.Id, QueryLevel.None);
            Assert.IsTrue(obj.IsEmpty());
        }

    }

}
