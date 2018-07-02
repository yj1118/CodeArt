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
    public class DataVersionTest5 : DomainStage
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

            Dog dog = CreateDog();

            var repository = Repository.Create<IDogRepository>();
            repository.Add(dog);

            this.Commit();

            this.Fixture.Add(dog);
        }

        #region 对象继承的测试

        [TestMethod]
        public void AnimalInheritedTest1()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.Name, "动物名称：一只动物");
            Assert.AreEqual(dogMemmory.NickName, "MyDog");
            Assert.AreEqual(dogMemmory.IsGood, true);
            Assert.AreEqual(dogMemmory.Age, 20);
            Assert.AreEqual(dogMemmory.LiveTime.Value, new DateTime(2017, 6, 11));
            Assert.AreEqual(dogMemmory.GetSignature(), "动物签名");
            Assert.AreEqual(dogMemmory.GetMyColors().Count, 2);
            Assert.AreEqual(dogMemmory.GetMainWheel().Description, "the main Wheel");
            Assert.AreEqual(dogMemmory.GetWheels().Count, 2);
            Assert.AreEqual(dogMemmory.GetMainBreak().Description, "MyAnimalBreak");
            Assert.AreEqual(dogMemmory.GetBreaks().Count, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest2()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.Age = 18;
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.Age, 18);
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest3()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.Name = "我是狗";
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.Name, "动物名称：我是狗");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest4()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.SetSignature("狗的签名");
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetSignature(), "狗的签名");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest5()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.GetMyColors().Add(new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty));
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetMyColors().Count, 3);
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest6()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.GetMainWheel().Description = "modify main Wheel";
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetMainWheel().Description, "modify main Wheel");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest7()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            AnimalWheel wheel3 = new AnimalWheel(4)
            {
                OrderIndex = 13,
                Description = "theAnimalWheel3",
                TheColor = new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            dog.GetWheels().Add(wheel3);

            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetWheels().Count, 3);

            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest8()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.GetWheels().ElementAt(0).Description = "modify theAnimalWheel1";
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetWheels().ElementAt(0).Description, "modify theAnimalWheel1");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest9()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.GetMainBreak().Description = "modify MyAnimalBreak";
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetMainBreak().Description, "modify MyAnimalBreak");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest10()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            AnimalBreak mybreak = new AnimalBreak(2)
            {
                Description = "other MainBreak",
                CreateDate = new DateTime(2017, 6, 10)
            };

            dog.SetMainBreak(mybreak);

            this.BeginTransaction();

            var repository = Repository.Create<IDogRepository>();
            repository.Update(dog);

            this.Commit();

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetMainBreak().Description, "other MainBreak");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest11()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            dog.GetWheels().ElementAt(0).Description = "modify theAnimalWheel1";
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dogMemmory.GetWheels().ElementAt(0).Description, "modify theAnimalWheel1");
            CheckDogVersion(dog.Id, 2);
        }

        [TestMethod]
        public void AnimalInheritedTest12()
        {
            var dog = this.Fixture.Get<Dog>() as Dog;

            CheckDogVersion(dog.Id, 1);

            // 修改
            AnimalWheel wheel3 = new AnimalWheel(4)
            {
                OrderIndex = 13,
                Description = "theAnimalWheel3",
                TheColor = new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            dog.GetWheels().Add(wheel3);
            UpdateDog(dog);

            Dog dogMemmory = GetDog(dog.Id);

            Assert.AreEqual(dog.GetWheels().Count, 3);

            CheckDogVersion(dog.Id, 2);
        }

        #endregion

        #region 测试工具

        private Dog CreateDog()
        {
            Dog dog = new Dog(Guid.NewGuid())
            {
                NickName = "MyDog",
                Age = 20,
                IsGood = true,
            };

            FillAnimal(dog);
            return dog;
        }

        private void FillAnimal(Animal animal)
        {
            animal.Name = "一只动物";
            animal.LiveTime = new DateTime(2017, 6, 1);

            animal.SetSignature("动物签名");

            List<AnimalColor> colors = new List<AnimalColor>();
            colors.Add(new AnimalColor("红色", 6, true, AnimalCategory.Empty, AnimalAccessory.Empty));
            colors.Add(new AnimalColor("蓝色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty));

            animal.SetMyColors(colors);

            AnimalWheel mainWheel = new AnimalWheel(30)
            {
                OrderIndex = 1,
                Description = "the main Wheel",
                TheColor = new AnimalColor("主色", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            animal.SetMainWheel(mainWheel);

            AnimalWheel wheel1 = new AnimalWheel(2)
            {
                OrderIndex = 11,
                Description = "theAnimalWheel1",
                TheColor = new AnimalColor("红色", 6, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            AnimalWheel wheel2 = new AnimalWheel(3)
            {
                OrderIndex = 12,
                Description = "theAnimalWheel2",
                TheColor = new AnimalColor("蓝色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            List<AnimalWheel> wheels = new List<AnimalWheel>();
            wheels.Add(wheel1);
            wheels.Add(wheel2);

            animal.SetWheels(wheels);

            AnimalBreak mainBreak = new AnimalBreak(1)
            {
                Description = "MyAnimalBreak",
                CreateDate = new DateTime(2017, 6, 1)
            };

            animal.SetMainBreak(mainBreak);

            AnimalBreak breaker1 = new AnimalBreak(11)
            {
                Description = "AnimalBreak1",
                CreateDate = new DateTime(2017, 6, 1)
            };

            AnimalBreak breaker2 = new AnimalBreak(12)
            {
                Description = "AnimalBreak2",
                CreateDate = new DateTime(2017, 6, 2)
            };

            List<AnimalBreak> breaks = new List<AnimalBreak>();
            AddAnimalBreak(breaker1, breaks);
            AddAnimalBreak(breaker2, breaks);

            animal.SetBreaks(breaks);
        }

        private void UpdateDog(Dog dog)
        {
            this.BeginTransaction();

            var repository = Repository.Create<IDogRepository>();
            repository.Update(dog);

            this.Commit();
        }

        private dynamic FindAnimal(Guid id)
        {
            dynamic animal = null;

            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from Animal where id=@id", new { Id = id });
                animal = data;
            });

            return animal;
        }

        private Dog GetDog(Guid id)
        {
            var repository = Repository.Create<IDogRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckDogVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from Animal where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        private void AddAnimalBreak(AnimalBreak mybreak, List<AnimalBreak> breaks)
        {
            breaks.Add(mybreak);
        }

        #endregion
    }
}
