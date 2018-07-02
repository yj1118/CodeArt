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
    public class DataVersionTest4 : DomainStage
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

            Animal animal = CreateAnimal();

            var repository = Repository.Create<IAnimalRepository>();
            repository.Add(animal);

            this.Commit();

            this.Fixture.Add(animal);
        }

        #region 对象扩展的测试

        [TestMethod]
        public void AnimalExtensionTest1()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;

            Assert.AreEqual(animal.Name, "动物名称：一只动物");

            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.Name = "修改的一只动物";
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animalNew.Name, "动物名称：修改的一只动物");

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest2()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;

            Assert.AreEqual(animal.LiveTime.Value, new DateTime(2017, 6, 11));

            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.LiveTime = new DateTime(2017, 9, 10);
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.LiveTime.Value, new DateTime(2017, 9, 20));

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest3()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;

            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetSignature("动物签名");
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetSignature(), "动物签名");

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest4()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;

            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetSignature("动物签名");
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetSignature(), "动物签名");

            CheckAnimalVersion(animal.Id, 2);

            // 二次修改
            animal.SetSignature("修改动物签名");
            UpdateAnimal(animal);

            animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetSignature(), "修改动物签名");

            CheckAnimalVersion(animal.Id, 3);


        }

        [TestMethod]
        public void AnimalExtensionTest5()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetIsEatMeet(), true);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetIsEatMeet(false);
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetIsEatMeet(), false);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest6()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetIsEatMeet(), true);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetIsEatMeet(false);
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetIsEatMeet(), false);

            CheckAnimalVersion(animal.Id, 2);

            // 二次修改
            animal.SetIsEatMeet(true);
            UpdateAnimal(animal);

            animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetIsEatMeet(), true);

            CheckAnimalVersion(animal.Id, 3);

        }

        [TestMethod]
        public void AnimalExtensionTest7()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetEyeCounts().Count, 0);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            List<decimal> eyes = new List<decimal>();
            eyes.Add(10);
            eyes.Add(11);

            animal.SetEyeCounts(eyes);
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetEyeCounts().Count, 2);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest8()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetEyeCounts().Count, 0);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            List<decimal> eyes = new List<decimal>();
            eyes.Add(10);
            eyes.Add(11);

            animal.SetEyeCounts(eyes);
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetEyeCounts().Count, 2);

            CheckAnimalVersion(animal.Id, 2);

            // 二次修改
            eyes.Remove(10);

            animal.SetEyeCounts(eyes);
            UpdateAnimal(animal);

            animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetEyeCounts().Count, 1);
            Assert.AreEqual(animal.GetEyeCounts().ElementAt(0), 11);

            CheckAnimalVersion(animal.Id, 3);

        }

        [TestMethod]
        public void AnimalExtensionTest9()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetTheColor(), AnimalColor.Empty);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetTheColor(new AnimalColor("蓝色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty));
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetTheColor().Name, "蓝色");

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest10()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetTheColor(), AnimalColor.Empty);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetTheColor(new AnimalColor("蓝色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty));
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetTheColor().Name, "蓝色");

            CheckAnimalVersion(animal.Id, 2);

            // 二次修改
            animal.SetTheColor(new AnimalColor("红色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty));
            UpdateAnimal(animal);

            animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetTheColor().Name, "红色");

            CheckAnimalVersion(animal.Id, 3);

        }

        [TestMethod]
        public void AnimalExtensionTest11()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetMyColors().Count, 2);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.GetMyColors().Add(new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty));

            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetMyColors().Count, 3);
            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest12()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetMyColors().Count, 2);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.GetMyColors().RemoveAt(0);

            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetMyColors().Count, 1);
            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest13()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetMyColors().Count, 2);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            List<AnimalColor> colors = new List<AnimalColor>();
            colors.Add(new AnimalColor("绿色",8, true, AnimalCategory.Empty, AnimalAccessory.Empty));

            animal.SetMyColors(colors);

            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetMyColors().Count, 1);
            Assert.AreEqual(animal.GetMyColors().ElementAt(0).Name, "绿色");


            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest14()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.GetMainWheel().Description = "modify main Wheel";

            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetMainWheel().Description, "modify main Wheel");

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest15()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            AnimalWheel wheel = new AnimalWheel(31)
            {
                OrderIndex = 1,
                Description = "modify Wheel",
                TheColor = new AnimalColor("蓝色", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            animal.SetMainWheel(wheel);

            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetMainWheel().Description, "modify Wheel");

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest16()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animalNew.MainBreakId, 1);

            // 修改
            animal.GetMainBreak().Description = "modify MyAnimalBreak";
            animal.GetMainBreak().CreateDate = new DateTime(2017, 6, 2);

            UpdateAnimal(animal);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetMainBreak().CreateDate.ToString(), new DateTime(2017, 6, 2).ToString());
            Assert.AreEqual(animalMemmory.GetMainBreak().Description, "modify MyAnimalBreak");

            animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);

            CheckAnimalBreakVersion(animal.GetMainBreak().Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest17()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animalNew.MainBreakId, 1);

            // 修改
            AnimalBreak mybreak = new AnimalBreak(2)
            {
                Description = "other MainBreak",
                CreateDate = new DateTime(2017, 6, 10)
            };

            animal.SetMainBreak(mybreak);

            this.BeginTransaction();

            var repository = Repository.Create<IAnimalRepository>();
            repository.Update(animal);

            this.Commit();

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetMainBreak().CreateDate.ToString(), new DateTime(2017, 6, 10).ToString());
            Assert.AreEqual(animalMemmory.GetMainBreak().Description, "other MainBreak");

            animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);

            CheckAnimalBreakVersion(animal.GetMainBreak().Id, 1);
        }

        [TestMethod]
        public void AnimalExtensionTest18()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().Count, 2);

            // 修改
            animal.GetWheels().ElementAt(0).Description = "modify theAnimalWheel1";
            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().ElementAt(0).Description, "modify theAnimalWheel1");

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest19()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().Count, 2);

            // 修改
            AnimalWheel wheel = animal.GetWheels().ElementAt(0);

            animal.GetWheels().Remove(wheel);
            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().Count, 1);

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest20()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().Count, 2);

            // 修改
            AnimalWheel wheel3 = new AnimalWheel(4)
            {
                OrderIndex = 13,
                Description = "theAnimalWheel3",
                TheColor = new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            animal.GetWheels().Add(wheel3);
            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetWheels().Count, 3);

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest21()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 2);

            // 修改
            var myBreak = animal.GetBreaks().ElementAt(0);
            myBreak.Description = "modify AnimalBreak1";

            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().ElementAt(0).Description, "modify AnimalBreak1");

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);

            CheckAnimalBreakVersion(animalMemmory.GetBreaks().ElementAt(0).Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest22()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 2);

            // 修改
            AnimalBreak myBreak = animal.GetBreaks().ElementAt(0);

            RemoveAnimalBreak(myBreak, animal.GetBreaks());

            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 1);

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest23()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 2);

            // 修改
            AnimalBreak breaker3 = new AnimalBreak(13)
            {
                Description = "AnimalBreak3",
                CreateDate = new DateTime(2017, 6, 3)
            };

            animal.GetBreaks().Add(breaker3);
            var repository = Repository.Create<IAnimalRepository>();

            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 3);

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalExtensionTest24()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 2);

            // 修改
            AnimalBreak breaker3 = new AnimalBreak(13)
            {
                Description = "AnimalBreak3",
                CreateDate = new DateTime(2017, 6, 3)
            };

            List<AnimalBreak> breaks = new List<AnimalBreak>();
            AddAnimalBreak(breaker3, breaks);

            animal.SetBreaks(breaks);

            UpdateAnimal(animal);

            animalMemmory = GetAnimal(animal.Id);

            Assert.AreEqual(animalMemmory.GetBreaks().Count, 1);
            Assert.AreEqual(animalMemmory.GetBreaks().ElementAt(0).Description, "AnimalBreak3");

            var animalNew = FindAnimal(animal.Id);

            CheckAnimalVersion(animal.Id, 2);
        }

        #endregion

        #region 测试工具

        private Animal CreateAnimal()
        {
            Animal animal = new Animal(Guid.NewGuid());

            animal.Name = "一只动物";
            animal.LiveTime = new DateTime(2017, 6, 1);

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

            return animal;
        }

        private void UpdateAnimal(Animal animal)
        {
            this.BeginTransaction();

            var repository = Repository.Create<IAnimalRepository>();
            repository.Update(animal);

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

        private Animal GetAnimal(Guid id)
        {
            var repository = Repository.Create<IAnimalRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckAnimalVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from Animal where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        private void CheckAnimalBreakVersion(long id, int dataVersion)
        {
            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from AnimalBreak where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        private void AddAnimalBreak(AnimalBreak mybreak, List<AnimalBreak> breaks)
        {
            breaks.Add(mybreak);
        }

        private void RemoveAnimalBreak(AnimalBreak mybreak, DomainCollection<AnimalBreak> breaks)
        {
            breaks.Remove(mybreak);
        }

        #endregion
    }
}
