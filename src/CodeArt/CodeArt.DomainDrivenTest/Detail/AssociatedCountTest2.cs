using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 在领域对象中，引用对象和值对象的引用计数测试
    /// </summary>
    [TestClass]
    public class AssociatedCountTest2 : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        #region 引用对象的引用计数测试

        [TestMethod]
        public void CarBrandDoorTest1()
        {
            CarBrand brand = CreateCarBrand(100);

            BrandDoor door = new BrandDoor(1){Name = "BrandDoor", OrderIndex = 10};

            brand.OneDoor = door;
            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.OneDoor = BrandDoor.Empty;
            UpdateCarBrand(brand);

            CheckExist(door.Id, false);

        }

        [TestMethod]
        public void CarBrandDoorTest2()
        {
            CarBrand brand = CreateCarBrand(100);

            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;
            brand.OtherDoor = door;

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.OneDoor = BrandDoor.Empty;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            CheckExist(door.Id, true);

        }

        [TestMethod]
        public void CarBrandDoorTest3()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;
            brand.OtherDoor = door;

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.OneDoor = BrandDoor.Empty;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            CheckExist(door.Id, true);

            brand.OtherDoor = BrandDoor.Empty;
            UpdateCarBrand(brand);

            CheckExist(door.Id, false);

        }

        [TestMethod]
        public void CarBrandDoorTest4()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.OtherDoor = door;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            CheckExist(door.Id, true);

        }

        [TestMethod]
        public void CarBrandDoorTest5()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.OtherDoor = door;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            CheckExist(door.Id, true);
        }

        [TestMethod]
        public void CarBrandDoorTest6()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.RemoveMyDoor(door);
            UpdateCarBrand(brand);

            CheckExist(door.Id, false);
        }

        [TestMethod]
        public void CarBrandDoorTest7()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);
            brand.AddMyDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.RemoveMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            CheckExist(door.Id, true);
        }

        [TestMethod]
        public void CarBrandDoorTest8()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.AddMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);
        }

        [TestMethod]
        public void CarBrandDoorTest9()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);
            brand.AddMyDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.RemoveMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.RemoveMyDoor(door);
            UpdateCarBrand(brand);

            CheckExist(door.Id, false);
        }

        [TestMethod]
        public void CarBrandDoorTest10()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);
            brand.AddOtherDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.RemoveMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.RemoveOtherDoor(door);
            UpdateCarBrand(brand);

            CheckExist(door.Id, false);
        }

        [TestMethod]
        public void CarBrandDoorTest11()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.AddMyDoor(door);
            brand.AddOtherDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.AddOtherDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 3);

            brand.RemoveOtherDoor(door);
            UpdateCarBrand(brand);
            CheckDoorAssociatedCount(door.Id, 2);

            CheckExist(door.Id, true);
        }

        [TestMethod]
        public void CarBrandDoorTest12()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;
            brand.AddOtherDoor(door);

            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.OneDoor = BrandDoor.Empty;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

        }

        [TestMethod]
        public void CarBrandDoorTest13()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;
            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.AddOtherDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.AddMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 3);

            brand.OtherDoor = door;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 4);

        }

        [TestMethod]
        public void CarBrandDoorTest14()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandDoor door = new BrandDoor(1) { Name = "BrandDoor", OrderIndex = 10 };

            brand.OneDoor = door;
            AddCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 1);

            brand.AddOtherDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.AddMyDoor(door);
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 3);

            brand.OtherDoor = door;
            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 4);

            brand.RemoveMyDoor(door);
            brand.OneDoor = BrandDoor.Empty;

            UpdateCarBrand(brand);

            CheckDoorAssociatedCount(door.Id, 2);

            brand.RemoveOtherDoor(door);
            brand.OtherDoor = BrandDoor.Empty;

            UpdateCarBrand(brand);

            CheckExist(door.Id, false);

        }

        #endregion

        #region 测试工具

        private CarBrand CreateCarBrand(int id)
        {
            CarBrand brand = new CarBrand(id);

            brand.Name = "著名品牌";
            brand.CreateDate = new DateTime(2016, 5, 1);

            return brand;
        }

        private void AddCarBrand(CarBrand brand)
        {
            this.BeginTransaction();

            var repository = Repository.Create<ICarBrandRepository>();
            repository.Add(brand);

            this.Commit();
        }

        private void UpdateCarBrand(CarBrand brand)
        {
            this.BeginTransaction();

            var repository = Repository.Create<ICarBrandRepository>();
            repository.Update(brand);
            
            this.Commit();
        }

        private void CheckDoorAssociatedCount(int id, int count)
        {
            DataPortal.Direct<CarBrand>((conn) =>
            {
                var data = conn.QuerySingle("select * from BrandDoor where id=@id", new { Id = id });
                Assert.AreEqual(data.AssociatedCount, count);
            });
        }

        private void CheckExist(int id, bool flag)
        {
            DataPortal.Direct<CarBrand>((conn) =>
            {
                var data = conn.Query("select * from BrandDoor where id=@id", new { Id = id });
                if (data.Count() == 0)
                    Assert.AreEqual(false, flag);
                else
                    Assert.AreEqual(true, flag);
            });
        }

        #endregion
    }

}
