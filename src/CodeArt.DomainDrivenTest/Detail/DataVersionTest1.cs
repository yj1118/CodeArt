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
    /// 在领域对象中，对象的属性以及子成员发生了改变，数据版本号都会改变
    /// </summary>
    [TestClass]
    public class DataVersionTest1 : DomainStage
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
            this.BeginTransaction();
            CarSlim carSlim = CreateCarSlim();

            var repository = Repository.Create<ICarSlimRepository>();
            repository.Add(carSlim);

            this.Commit();

            this.Fixture.Add(carSlim);
        }

        #region 简单根CarSlim测试

        [TestMethod]
        public void CarSlimTest1()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.CarStyle = CarStyle.BUS;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((CarStyle)carSlimNew.CarStyle, CarStyle.BUS);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest2()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.PartNum = 130;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((byte)carSlimNew.PartNum, 130);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest3()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.Stand = 'A';

            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            //Assert.AreEqual((char)carSlimNew.Stand, 'A');

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest4()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.Name = "奥迪A6";
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual(carSlimNew.Name, "奥迪A6");

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest5()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.DeliveryDate = new Emptyable<DateTime>(new DateTime(2016, 7, 8));
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((DateTime)carSlimNew.DeliveryDate, new DateTime(2016, 7, 8));

            CarSlim carMemmory = GetCarSlim(carSlim.Id);

            Assert.AreEqual(carMemmory.DeliveryDate.GetType().ToString(), typeof(Emptyable<DateTime>).ToString());

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest6()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.OrderIndex = 22;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((int)carSlimNew.OrderIndex, 22);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest7()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.Distance = 4.89f;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((float)carSlimNew.Distance, 4.89f);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest8()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.Price = 48.96m;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((decimal)carSlimNew.Price, 49m);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest9()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.IsNewCar = false;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((bool)carSlimNew.IsNewCar, false);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        [TestMethod]
        public void CarSlimTest10()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);

            carSlim.IsNewCar = true;
            carSlim.OrderIndex = 11;
            carSlim.Distance = 3.56f;
            carSlim.Price = 34.85m;

            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);

            Assert.AreEqual((bool)carSlimNew.IsNewCar, true);
            Assert.AreEqual((int)carSlimNew.OrderIndex, 11);
            Assert.AreEqual((float)carSlimNew.Distance, 3.56f);
            Assert.AreEqual((decimal)carSlimNew.Price, 35m);

            CheckCarSlimDataVersion(carSlim.Id, 1);
        }

        [TestMethod]
        public void CarSlimTest11()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);

            carSlim.IsNewCar = false;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((bool)carSlimNew.IsNewCar, false);

            CheckCarSlimDataVersion(carSlim.Id, 2);

            carSlim.CarStyle = CarStyle.JEEP;
            UpdateCarSlim(carSlim);

            var carSlimNew2 = FindCarSlim(carSlim.Id);
            Assert.AreEqual((CarStyle)carSlimNew2.CarStyle, CarStyle.JEEP);

            CheckCarSlimDataVersion(carSlim.Id, 3);

        }

        [TestMethod]
        public void CarSlimTest12()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);

            carSlim.IsNewCar = false;
            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((bool)carSlimNew.IsNewCar, false);

            CheckCarSlimDataVersion(carSlim.Id, 2);

            carSlim.IsNewCar = true;
            UpdateCarSlim(carSlim);

            var carSlimNew2 = FindCarSlim(carSlim.Id);
            Assert.AreEqual((bool)carSlimNew2.IsNewCar, true);

            CheckCarSlimDataVersion(carSlim.Id, 3);

        }

        [TestMethod]
        public void CarSlimTest13()
        {
            var carSlim = this.Fixture.Get<CarSlim>() as CarSlim;
            CheckCarSlimDataVersion(carSlim.Id, 1);
            carSlim.Comment = 12680;

            UpdateCarSlim(carSlim);

            var carSlimNew = FindCarSlim(carSlim.Id);
            Assert.AreEqual((short)carSlimNew.Comment, 12680);

            CheckCarSlimDataVersion(carSlim.Id, 2);
        }

        #endregion

        #region CarSlim测试工具

        private CarSlim CreateCarSlim()
        {
            CarSlim carSlim = new CarSlim(Guid.NewGuid());

            carSlim.CarStyle = CarStyle.SEDAN;
            carSlim.PartNum = 128;
            carSlim.Stand = 'B';
            carSlim.Comment = 32600;
            carSlim.Name = "宝马i7";
            carSlim.DeliveryDate = new Emptyable<DateTime>(new DateTime(2016, 5, 1));
            carSlim.OrderIndex = 11;
            carSlim.Distance = 3.56f;
            carSlim.Price = 34.85m;
            carSlim.IsNewCar = true;

            return carSlim;
        }

        private void UpdateCarSlim(CarSlim carSlim)
        {
            this.BeginTransaction();

            var repository = Repository.Create<ICarSlimRepository>();
            repository.Update(carSlim);
            
            this.Commit();
        }

        private dynamic FindCarSlim(Guid id)
        {
            dynamic carSlim = null;

            DataPortal.Direct<CarSlim>((conn) =>
            {
                var data = conn.QuerySingle("select * from CarSlim where id=@id", new { Id = id });
                carSlim = data;
            });

            return carSlim;
        }

        private CarSlim GetCarSlim(Guid id)
        {
            var repository = Repository.Create<ICarSlimRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckCarSlimDataVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<CarSlim>((conn) =>
            {
                var data = conn.QuerySingle("select * from CarSlim where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        #endregion
    }

}
