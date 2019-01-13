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
using CodeArt.ServiceModel;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 在领域对象中，对象的属性以及子成员发生了改变，数据版本号都会改变
    /// </summary>
    [TestClass]
    public class DataVersionTest3 : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();

            //恢复原来的远程对象
            ServiceContext.InvokeDynamic("UpdateUser", (arg) =>
            {
                arg.Id = 1;
                arg.Name = "袁俊";
            });

            // 删除远程对象
            ServiceContext.InvokeDynamic("DeleteUser", (arg) =>
            {
                arg.Id = 20;
            });

        }

        protected override void EnteredScene()
        {
            this.BeginTransaction();
            Car car = CreateCar();

            var repository = Repository.Create<ICarRepository>();
            repository.Add(car);

            this.Commit();

            this.Fixture.Add(car);
        }

        #region Car中有远程对象的测试

        [TestMethod]
        public void CarRemoteTest2()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.Owner = Repository.FindRemoteRoot<CarUser>(1);

            Assert.AreEqual("袁俊", car.Owner.Name);
        }

        [TestMethod]
        public void CarRemoteTest1()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.Owner = Repository.FindRemoteRoot<CarUser>(1);

            Assert.AreEqual(car.Owner.Name, "袁俊");
            int dv = GetCarUserVersion(1);

            // 修改远程对象
            ServiceContext.InvokeDynamic("UpdateUser",(arg) =>
            {
                arg.Id = 1;
                arg.Name = "袁俊修改";
            });

            System.Threading.Thread.Sleep(2000);

            car.Owner = Repository.FindRemoteRoot<CarUser>(1);

            Assert.AreEqual("袁俊修改", car.Owner.Name);
            CheckCarDataVersion(car.Id, 1);


            // 修改远程对象
            ServiceContext.InvokeDynamic("UpdateUser", (arg) =>
            {
                arg.Id = 1;
                arg.Name = "袁俊";
            });
            System.Threading.Thread.Sleep(2000);
        }

        [TestMethod]
        public void CarRemoteTest3()
        {
            // 创建一个远程对象
            ServiceContext.InvokeDynamic("CreateUser", (arg) =>
            {
                arg.Id = 20;
                arg.Name = "袁俊新增";
            });

            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.Owner = Repository.FindRemoteRoot<CarUser>(20);

            Assert.AreEqual(car.Owner.Name, "袁俊新增");
            int dv = GetCarUserVersion(20);

            // 删除远程对象
            ServiceContext.InvokeDynamic("DeleteUser", (arg) =>
            {
                arg.Id = 20;
            });

            System.Threading.Thread.Sleep(2000);

            Assert.AreEqual("袁俊新增", car.Owner.Name);

            car.Owner = Repository.FindRemoteRoot<CarUser>(20);
            CheckCarDataVersion(car.Id, 1);

            Assert.AreEqual("", car.Owner.Name);
        }

        [TestMethod]
        public void CarRemoteTest4()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.Owner = Repository.FindRemoteRoot<CarUser>(1);

            UpdateCar(car);

            CheckCarDataVersion(car.Id, 2);

        }

        [TestMethod]
        public void CarRemoteTest5()
        {
            var car = this.Fixture.Get<Car>() as Car;
            CheckCarDataVersion(car.Id, 1);

            car.Owner = Repository.FindRemoteRoot<CarUser>(1);

            UpdateCar(car);

            CheckCarDataVersion(car.Id, 2);
            var carNew = FindCar(car.Id);
            Assert.AreEqual(1, carNew.OwnerId);

            car.Owner = Repository.FindRemoteRoot<CarUser>(10);
            UpdateCar(car);

            CheckCarDataVersion(car.Id, 3);
            var carNew2 = FindCar(car.Id);
            Assert.AreEqual(10, carNew2.OwnerId);
        }

        #endregion

        #region Car测试工具

        private Car CreateCar()
        {
            Car car = new Car(Guid.NewGuid());

            car.Name = "宝马i7";
            car.IsNewCar = true;

            car.LightCounts.Add(1);
            car.LightCounts.Add(2);

            car.ErrorMessages = new List<string> { "error1", "error2" };

            car.DeliveryDates = new List<Emptyable<DateTime>> {
                new DateTime(2016, 6, 1),
                new DateTime(2016, 6, 2),
                new DateTime(2016, 6, 3)
            };

            car.AllColor = new WholeColor("颜色", 8, true);

            List<CarAccessory> accessories = new List<CarAccessory>()
            {
                new CarAccessory("配饰1", 10, new DateTime(2017, 6, 1)),
                new CarAccessory("配饰2", 11, new DateTime(2017, 6, 2)),
                new CarAccessory("配饰3", 12, new DateTime(2017, 6, 3))
            };

            car.CarAccessories = accessories;

            car.MainWheel = new CarWheel(1)
            {
                OrderIndex = 1,
                Description = "the main CarWheel",
                TheColor = new WholeColor("主色", 5, true)
            };

            CarWheel wheel1 = new CarWheel(2)
            {
                OrderIndex = 11,
                Description = "theCarWheel1",
                TheColor = new WholeColor("红色", 6, true)
            };

            CarWheel wheel2 = new CarWheel(3)
            {
                OrderIndex = 12,
                Description = "theCarWheel2",
                TheColor = new WholeColor("蓝色", 7, true)
            };

            car.AddCarWheel(wheel1);
            car.AddCarWheel(wheel2);

            car.MainBreak = new CarBreak(1)
            {
                Description = "MainBreak",
                TheColor = new WholeColor("主色", 5, true),
                CreateDate = new DateTime(2017, 6, 1)
            };


            return car;
        }

        private void UpdateCar(Car car)
        {
            this.BeginTransaction();

            var repository = Repository.Create<ICarRepository>();
            repository.Update(car);

            this.Commit();
        }

        private dynamic FindCar(Guid id)
        {
            dynamic car = null;
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from Car where id=@id", new { Id = id });
                car = data;
            });

            return car;
        }

        private Car GetCar(Guid id)
        {
            var repository = Repository.Create<ICarRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckCarDataVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from Car where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        private int GetCarUserVersion(int id)
        {
            int dv = 0;
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from CarUser where id=@id", new { Id = id });
                dv = data.DataVersion;
            });

            return dv;
        }

        #endregion
    }
}
