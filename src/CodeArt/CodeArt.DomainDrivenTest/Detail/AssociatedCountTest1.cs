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
    public class AssociatedCountTest1 : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        #region 值对象的引用计数测试

        [TestMethod]
        public void CarBrandTest1()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.BrandAccessory = BrandAccessory.Empty;
            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);

        }

        [TestMethod]
        public void CarBrandTest2()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            brand.OtherAccessory = acc;

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.BrandAccessory = BrandAccessory.Empty;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            CheckExist(acc.Id, true);

        }

        [TestMethod]
        public void CarBrandTest3()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            brand.OtherAccessory = acc;

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.BrandAccessory = BrandAccessory.Empty;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            CheckExist(acc.Id, true);

            brand.OtherAccessory = BrandAccessory.Empty;
            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);

        }

        [TestMethod]
        public void CarBrandTest4()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.OtherAccessory = acc;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            CheckExist(acc.Id, true);
        }

        [TestMethod]
        public void CarBrandTest5()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.OtherAccessory = acc;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            CheckExist(acc.Id, true);
        }

        [TestMethod]
        public void CarBrandTest6()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.AddMyAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);
        }

        [TestMethod]
        public void CarBrandTest7()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.AddMyAccessory(acc);
            brand.AddMyAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            CheckExist(acc.Id, true);
        }

        [TestMethod]
        public void CarBrandTest8()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.AddMyAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.AddMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);
        }

        [TestMethod]
        public void CarBrandTest9()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));


            brand.AddMyAccessory(acc);
            brand.AddMyAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);
        }

        [TestMethod]
        public void CarBrandTest10()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.AddMyAccessory(acc);
            brand.AddYouAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.RemoveYouAccessory(acc);
            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);
        }

        [TestMethod]
        public void CarBrandTest11()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.AddMyAccessory(acc);
            brand.AddMyAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.AddYouAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 3);

            brand.RemoveMyAccessory(acc);
            UpdateCarBrand(brand);
            CheckAccessoryAssociatedCount(acc.Id, 2);

            CheckExist(acc.Id, true);
        }

        [TestMethod]
        public void CarBrandTest12()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            brand.AddYouAccessory(acc);

            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.BrandAccessory = BrandAccessory.Empty;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

        }

        [TestMethod]
        public void CarBrandTest13()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.AddYouAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.AddMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 3);

            brand.OtherAccessory = acc;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 4);

        }

        [TestMethod]
        public void CarBrandTest14()
        {
            CarBrand brand = CreateCarBrand(100);
            BrandAccessory acc = new BrandAccessory("CarBrand配饰", 10, new DateTime(2017, 6, 1));

            brand.BrandAccessory = acc;
            AddCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            brand.AddYouAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.AddMyAccessory(acc);
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 3);

            brand.OtherAccessory = acc;
            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 4);

            brand.RemoveMyAccessory(acc);
            brand.OtherAccessory = BrandAccessory.Empty;

            UpdateCarBrand(brand);

            CheckAccessoryAssociatedCount(acc.Id, 2);

            brand.RemoveYouAccessory(acc);
            brand.BrandAccessory = BrandAccessory.Empty;

            UpdateCarBrand(brand);

            CheckExist(acc.Id, false);

        }

        [TestMethod]
        public void CarBrandTest15()
        {
            BrandAccessory acc = new BrandAccessory("CarBrand1配饰", 10, new DateTime(2017, 6, 1));

            CarBrand brand1 = CreateCarBrand(100);

            brand1.BrandAccessory = acc;
            AddCarBrand(brand1);

            CheckAccessoryAssociatedCount(acc.Id, 1);

            CarBrand brand2 = CreateCarBrand(101);

            brand2.BrandAccessory = acc;
            AddCarBrand(brand2);

            //CheckAccessoryAssociatedCount(acc.Id, 2);

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

        private void CheckAccessoryAssociatedCount(Guid id, int count)
        {
            DataPortal.Direct<CarBrand>((conn) =>
            {
                var data = conn.QuerySingle("select * from BrandAccessory where id=@id", new { Id = id });
                Assert.AreEqual(data.AssociatedCount, count);
            });
        }

        private void CheckExist(Guid id, bool flag)
        {
            DataPortal.Direct<CarBrand>((conn) =>
            {
                var data = conn.Query("select * from BrandAccessory where id=@id", new { Id = id });
                if (data.Count() == 0)
                    Assert.AreEqual(false, flag);
                else
                    Assert.AreEqual(true, flag);
            });
        }

        #endregion
    }

}
