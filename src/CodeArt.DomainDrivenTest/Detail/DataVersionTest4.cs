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

        #region 对象扩展与继承的测试

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
            Assert.AreEqual(animal.GetTheColor(), WholeColor.Empty);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetTheColor(new AnimalColor("蓝色", 7, true));
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetTheColor().Name, "蓝色");

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalExtensionTest10()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            Assert.AreEqual(animal.GetTheColor(), WholeColor.Empty);
            CheckAnimalVersion(animal.Id, 1);

            // 修改
            animal.SetTheColor(new AnimalColor("蓝色", 7, true));
            UpdateAnimal(animal);

            var animalNew = FindAnimal(animal.Id);
            Assert.AreEqual(animal.GetTheColor().Name, "蓝色");

            CheckAnimalVersion(animal.Id, 2);

            // 二次修改
            animal.SetTheColor(new AnimalColor("红色", 7, true));
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
            animal.GetMyColors().Add(new AnimalColor("绿色", 8, true));

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
            colors.Add(new AnimalColor("绿色",8, true));

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
            //var animal = this.Fixture.Get<Animal>() as Animal;
            //Assert.AreEqual(animal.GetMainWheel().Description, "the main CarWheel");
            //CheckAnimalVersion(animal.Id, 1);

            //// 修改
            //animal.GetMainWheel().Description = "modify main CarWheel";

            //UpdateAnimal(animal);

            //var animalNew = FindAnimal(animal.Id);
            //Assert.AreEqual(animal.GetMainWheel().Description, "modify main CarWheel");

            //CheckAnimalVersion(animal.Id, 2);
        }

        #endregion

        #region 测试工具

        private Animal CreateAnimal()
        {
            Animal animal = new Animal(Guid.NewGuid());

            animal.Name = "一只动物";
            animal.LiveTime = new DateTime(2017, 6, 1);

            List<AnimalColor> colors = new List<AnimalColor>();
            colors.Add(new AnimalColor("红色", 6, true));
            colors.Add(new AnimalColor("蓝色", 7, true));

            animal.SetMyColors(colors);

            CarWheel mainWheel = new CarWheel(30)
            {
                OrderIndex = 1,
                Description = "the main CarWheel",
                TheColor = new WholeColor("主色", 5, true)
            };

            animal.SetMainWheel(mainWheel);

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
            DataPortal.Direct<Car>((conn) =>
            {
                var data = conn.QuerySingle("select * from Animal where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        #endregion
    }
}
