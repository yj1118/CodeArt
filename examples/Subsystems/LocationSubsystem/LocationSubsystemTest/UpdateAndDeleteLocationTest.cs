using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using LocationSubsystem;

namespace LocationSubsystemTest
{
    [TestClass]
    public class UpdateAndDeleteLocationTest : DomainStage
    {
        protected override void PreEnterScene()
        {
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


        /// <summary>
        /// 简单的修改
        /// </summary>
        [TestMethod]
        public void UpdateWithSimple()
        {
            Location location = Util.CreateLocation("湖北省");

            var cmd = new UpdateLocation(location.Id)
            {
                Name = "武汉",
                SortNumber = 1,
                MarkedCode = "wuhan"
            };
            cmd.Execute();
            Util.AssertLocation(location.Id, (name: "武汉", markedCode: "wuhan", sortNumber: 1, parentId: 0, childCount: 0));
        }

        /// <summary>
        /// 当一个地理位置有父亲也有子项时，对它的修改
        /// </summary>
        [TestMethod]
        public void UpdateWithParentAndChild()
        {
            Location chinese = Util.CreateLocation("中国");
            Location hubei = Util.CreateLocation("湖北省", chinese.Id);
            Location wuhan = Util.CreateLocation("武汉", hubei.Id);
            Location xiaogan = Util.CreateLocation("孝感", hubei.Id);
            Location hunan = Util.CreateLocation("湖南省", chinese.Id);
           
            {
                var cmd = new UpdateLocation(hunan.Id)
                {
                    Name = "长沙",
                    SortNumber = 1,
                    MarkedCode = "changsha",
                };
                cmd.Execute();
                Util.AssertLocation(hunan.Id, (name: "长沙", markedCode: "changsha", sortNumber: 1, parentId: chinese.Id, childCount: 0));
            }
            {
                var cmd = new UpdateLocation(hubei.Id)
                {
                    Name = "鄂州",
                    SortNumber = 3,
                    MarkedCode = "ezhou",
                };
                cmd.Execute();
                Util.AssertLocation(hubei.Id, (name: "鄂州", markedCode: "ezhou", sortNumber: 3, parentId: chinese.Id, childCount: 2));
            }
            {
                var cmd = new UpdateLocation(hubei.Id)
                {
                    Name = "天心区",
                    SortNumber = 0,
                    MarkedCode = "tianxinqu",
                };
                cmd.Execute();
                Util.AssertLocation(hubei.Id, (name: "天心区", markedCode: "tianxinqu", sortNumber: 0, parentId: chinese.Id, childCount: 2));
            }
        }

        /// <summary>
        /// 要修改成的地理位置的名称在数据库中已经存在时，对他的修改
        /// </summary>
        [TestMethod]
        public void UpdateWithExistName()
        {
            Location chinese = Util.CreateLocation("中国");
            Location hubei = Util.CreateLocation("湖北省", chinese.Id);
            Location wuhan = Util.CreateLocation("武汉", hubei.Id);
            Location xiaogan = Util.CreateLocation("孝感", hubei.Id);
            Location hunan = Util.CreateLocation("湖南省", chinese.Id);
            {
                var cmd = new UpdateLocation(hubei.Id)

                {
                    Name = "中国",
                    SortNumber = 0,
                    MarkedCode = "chinese",
                };
                cmd.Execute();
                Util.AssertLocation(hubei.Id, (name: "中国", markedCode: "chinese", sortNumber: 0, parentId: chinese.Id, childCount: 2));
            }
            {
                var cmd = new UpdateLocation(hubei.Id)

                {
                    Name = "武汉",
                    SortNumber = 0,
                    MarkedCode = "wuhan",
                };
                cmd.Execute();
                Util.AssertLocation(hubei.Id, (name: "武汉", markedCode: "wuhan", sortNumber: 0, parentId: chinese.Id, childCount: 2));
            }
            {
                var cmd = new UpdateLocation(hubei.Id)
                {
                    Name = "湖南省",
                    SortNumber = 0,
                    MarkedCode = "hunan",
                };
                cmd.Execute();
                Util.AssertLocation(hubei.Id, (name: "湖南省", markedCode: "hunan", sortNumber: 0, parentId: chinese.Id, childCount: 2));
            }
            {
                var cmd = new UpdateLocation(hunan.Id)
                {
                    Name = "孝感",
                    SortNumber = 2,
                    MarkedCode = "xiaogan",
                };
                cmd.Execute();
                Util.AssertLocation(hunan.Id, (name: "孝感", markedCode: "xiaogan", sortNumber: 2, parentId: chinese.Id, childCount: 0));
            }
        }
        /// <summary>
        /// 简单的删除
        /// </summary>

        [TestMethod]
        public void DeleteWithSimple()
        {
            Location location = Util.CreateLocation("武汉");
            var cmd = new DeleteLocation(location.Id);
            cmd.Execute();
            Util.AssertNotExistLocation(location.Id);
        }
        /// <summary>
        /// 当一个地理位置有父项也有子项时，对它的删除
        /// 删除带子项的地理位置，其子项也会被删除
        /// </summary>
        [TestMethod]
        public void DeleteWithParentAndChild()
        {
            Location chinese = Util.CreateLocation("中国");
            Location hubei = Util.CreateLocation("湖北省", chinese.Id);
            Location wuhan = Util.CreateLocation("武汉", hubei.Id);
            Location hanyang = Util.CreateLocation("汉阳区", wuhan.Id);

            var hubeiLR = Util.GetLocationLR(hubei.Id);

            var count = Util.GetLocationAllChildsCount(hubeiLR);
            Assert.IsTrue(count == 2);

            var cmd = new DeleteLocation(hubei.Id);
            cmd.Execute();

            count = Util.GetLocationAllChildsCount(hubeiLR);
            Assert.AreEqual(0, count);

            Util.AssertNotExistLocation(hubei.Id);
            Util.AssertNotExistLocation(wuhan.Id);
            Util.AssertNotExistLocation(hanyang.Id);

        }

    }

}
