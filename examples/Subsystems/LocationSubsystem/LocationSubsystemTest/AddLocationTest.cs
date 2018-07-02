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
    public class AddLocationTest : DomainStage
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


        /// <summary>
        /// 简单的添加地理位置的测试，不包括父地理位置和子地理位置
        /// </summary>
        [TestMethod]
        public void AddWithSimple()
        {
            Location wuhan = Util.CreateLocation("武汉", "wuhan", 1);
            Util.AssertLocation(wuhan.Id, ("武汉", "wuhan", 1, 0, 0));
        }

        /// <summary>
        /// 添加一个包括父地理位置的地理位置
        /// </summary>
        [TestMethod]
        public void AddWithParent()
        {
            Location hubei = Util.CreateLocation("湖北省");
            Location wuhan = Util.CreateLocation("武汉", "wuhan", 1, hubei.Id);
            Util.AssertLocation(wuhan.Id, ("武汉", "wuhan", 1, hubei.Id, 0));
            Util.AssertLocation(hubei.Id, ("湖北省", "", 0, 0, 1));

        }

        [TestMethod]
        public void AddWithChilds()
        {
            Location hubei = Util.CreateLocation("湖北省");

            Location wuhan = Util.CreateLocation("武汉", "wuhan", 1, hubei.Id);
            Util.AssertLocation(wuhan.Id, ("武汉", "wuhan", 1, hubei.Id,0));

            Location xiaogan = Util.CreateLocation("孝感", "", 2, hubei.Id);
            Util.AssertLocation(xiaogan.Id, ("孝感", "", 2, hubei.Id, 0));

            Util.AssertLocation(hubei.Id, ("湖北省", "", 0, 0, 2));
        }

        /// <summary>
        /// 多个地理位置、多层级的复杂添加操作
        /// </summary>
        [TestMethod]
        public void AddWithComplicated()
        {
            //中国 -> 湖北省，湖南省
            //中国 -> 湖北省 ->武汉，孝感，鄂州
            //中国 -> 湖南省 -> 长沙 -> 天心区

            Location chinese = Util.CreateLocation("中国");
            Util.AssertLocationPath(string.Empty, chinese);

            Location hubei = Util.CreateLocation("湖北省", chinese.Id);

            Location wuhan = Util.CreateLocation("武汉", "wuhan", 1, hubei.Id);
            Util.AssertLocation(wuhan.Id, (name : "武汉", 
                                      markedCode : "wuhan", 
                                      sortNumber : 1, 
                                      parentId: hubei.Id, 
                                      childCount: 0));

            Util.AssertLocationPath("中国 > 湖北省", wuhan);

            Location xiaogan = Util.CreateLocation("孝感", "", 2, hubei.Id);
            Util.AssertLocation(xiaogan.Id, ("孝感", "", 2, hubei.Id, 0));

            Location ezhou = Util.CreateLocation("鄂州","ezhou",3, hubei.Id);
            Util.AssertLocation(ezhou.Id, ("鄂州", "ezhou", 3, hubei.Id, 0));


            Location hunan = Util.CreateLocation("湖南省", "hunan", 2, chinese.Id);
            Location changsha = Util.CreateLocation("长沙", "changsha", 1, hunan.Id);

            Location tianxin = Util.CreateLocation("天心区", changsha.Id);
            Util.AssertLocation(tianxin.Id, ("天心区", "", 0, changsha.Id,0));

            Util.AssertLocation(changsha.Id, ("长沙", "changsha", 1, hunan.Id,1));


            Util.AssertLocation(chinese.Id, (name: "中国", markedCode: "", sortNumber: 0, parentId: 0, childCount: 2));
            Util.AssertLocation(hubei.Id, ("湖北省", "",0, chinese.Id, 3));
            Util.AssertLocation(hunan.Id, ("湖南省", "hunan", 2, chinese.Id, 1));
            Util.AssertLocation(changsha.Id, ("长沙", "changsha", 1, hunan.Id, 1));

            Util.AssertLocationAllChilds(chinese.Id, 7);
            Util.AssertLocationAllChilds(hunan.Id, 2);
        }


     

    }
}
