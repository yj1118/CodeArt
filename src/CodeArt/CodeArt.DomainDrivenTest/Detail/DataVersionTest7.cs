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
    public class DataVersionTest7 : DomainStage
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

        #region 复杂混合对象的测试

        [TestMethod]
        public void AnimalComplexTest1()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.Category.Name, "爬行类");

            // 修改
            UpdateAnimalCategory(animal.Category.Id, "爬行类修改", new DateTime(2017, 6, 3));
            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.Category.Name, "爬行类修改");

            CheckAnimalVersion(animal.Id, 1);
            CheckAnimalCategoryVersion(aniMemmory.Category.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest2()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.Category.Name, "爬行类");

            // 修改
            AnimalCategory category = InsertAnimalCategory(2, "灵长类", new DateTime(2017, 6, 3));
            animal.Category = category;

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.Category.Name, "灵长类");

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest3()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMyColors().ElementAt(0).Category.Name, "红色动物类");

            // 修改
            AnimalCategory category = animal.GetMyColors().ElementAt(0).Category;
            UpdateAnimalCategory(category.Id, "红色动物类修改", new DateTime(2017, 6, 3));

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMyColors().ElementAt(0).Category.Name, "红色动物类修改");

            CheckAnimalVersion(animal.Id, 1);
            CheckAnimalCategoryVersion(category.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest4()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllColor.Category.Name, "AllColor动物类");

            // 修改
            AnimalCategory category = InsertAnimalCategory(6, "AllColor灵长类", new DateTime(2017, 6, 6));
            animal.AllColor = new AnimalColor("AllColor", 6, true, category, AnimalAccessory.Empty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllColor.Category.Name, "AllColor灵长类");

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest5()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Category.Name, "main Wheel动物类");

            // 修改
            UpdateAnimalCategory(animal.GetMainWheel().Category.Id, "main Wheel动物类修改", new DateTime(2017, 6, 6));

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Category.Name, "main Wheel动物类修改");

            CheckAnimalVersion(animal.Id, 1);
            CheckAnimalCategoryVersion(aniMemmory.GetMainWheel().Category.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest6()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Category.Name, "main Wheel动物类");

            // 修改
            AnimalCategory category = InsertAnimalCategory(7, "main Wheel灵长类", new DateTime(2017, 6, 6));
            animal.GetMainWheel().Category = category;

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Category.Name, "main Wheel灵长类");

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest7()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Category.Name, "AllAnimalBreak动物类");

            // 修改
            UpdateAnimalCategory(aniMemmory.AllBreak.Category.Id, "AllAnimalBreak动物类修改", new DateTime(2017, 6, 8));

            var repository = Repository.Create<IAnimalRepository>();
            repository.Update(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllAnimalBreak动物类修改", aniMemmory.AllBreak.Category.Name);
            Assert.AreEqual(false, aniMemmory.AllBreak.Category.IsSnapshot);

            CheckAnimalVersion(animal.Id, 1);
            CheckAnimalBreakVersion(aniMemmory.AllBreak.Id, 1);
            CheckAnimalCategoryVersion(aniMemmory.AllBreak.Category.Id, 2);

            DeleteAnimalCategory(aniMemmory.AllBreak.Category);

            Assert.AreEqual("AllAnimalBreak动物类修改", aniMemmory.AllBreak.Category.Name);
            Assert.AreEqual(true, aniMemmory.AllBreak.Category.IsSnapshot);
        }

        [TestMethod]
        public void AnimalComplexTest8()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Category.Name, "AllAnimalBreak动物类");

            // 修改
            AnimalCategory category = InsertAnimalCategory(9, "AllAnimalBreak灵长类", new DateTime(2017, 6, 9));
            animal.AllBreak.Category = category;

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Category.Name, "AllAnimalBreak灵长类");

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalBreakVersion(aniMemmory.AllBreak.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest9()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainBreak().Category.Name, "MyAnimalBreak动物类");

            // 修改
            UpdateAnimalCategory(animal.GetMainBreak().Category.Id, "MyAnimalBreak动物类修改", new DateTime(2017, 6, 6));

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainBreak().Category.Name, "MyAnimalBreak动物类修改");
            Assert.AreEqual(false, aniMemmory.GetMainBreak().Category.IsSnapshot);

            CheckAnimalVersion(animal.Id, 1);
            CheckAnimalCategoryVersion(aniMemmory.GetMainBreak().Category.Id, 2);
            CheckAnimalBreakVersion(aniMemmory.GetMainBreak().Id, 1);
        }

        [TestMethod]
        public void AnimalComplexTest10()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainBreak().Category.Name, "MyAnimalBreak动物类");

            // 修改
            AnimalCategory category = InsertAnimalCategory(8, "MyAnimalBreak灵长类", new DateTime(2017, 6, 9));
            animal.GetMainBreak().Category = category;

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainBreak().Category.Name, "MyAnimalBreak灵长类");

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalBreakVersion(aniMemmory.GetMainBreak().Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest11()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllColor.Accessory.Name, "AllColor配饰");

            // 修改
            animal.AllColor = new AnimalColor("颜色", 8, true, AnimalCategory.Empty,
                    new AnimalAccessory("AllColor配饰修改", 10, new DateTime(2017, 6, 10), AnimalDoor.Empty, AnimalEye.Empty));

            UpdateAnimal(animal);


            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllColor配饰修改", aniMemmory.AllColor.Accessory.Name);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest12()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Accessory.Name, "MainWheel配饰");

            // 修改
            animal.GetMainWheel().Accessory = new AnimalAccessory("MainWheel配饰修改", 10, new DateTime(2017, 6, 10), AnimalDoor.Empty, AnimalEye.Empty);
            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("MainWheel配饰修改", aniMemmory.GetMainWheel().Accessory.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest13()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainBreak().Accessory.Name, "MyAnimalBreak配饰");

            // 修改
            animal.GetMainBreak().Accessory = new AnimalAccessory("MyAnimalBreak配饰修改", 10, new DateTime(2017, 6, 10), AnimalDoor.Empty, AnimalEye.Empty);
            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("MyAnimalBreak配饰修改", aniMemmory.GetMainBreak().Accessory.Name);

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalBreakVersion(animal.GetMainBreak().Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest14()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllAccessory.Door.Name, "AllAccessoryDoor");

            // 修改
            animal.AllAccessory.Door.Name = "AllAccessoryDoor修改";
            Assert.AreEqual(true, animal.AllAccessory.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllAccessoryDoor修改", aniMemmory.AllAccessory.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest15()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllAccessory.Door.Name, "AllAccessoryDoor");

            // 修改
            AnimalDoor door = new AnimalDoor(50) { Name = "NewAllAccessoryDoor" };

            animal.AllAccessory = new AnimalAccessory("NewAllAccessory", 8, new DateTime(2017, 6, 8), door, AnimalEye.Empty);

            Assert.AreEqual(true, animal.AllAccessory.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllAccessoryDoor", aniMemmory.AllAccessory.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest16()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllColor.Accessory.Door.Name, "AllColor配饰Door");

            // 修改
            animal.AllColor.Accessory.Door.Name = "AllColor配饰Door修改";
            Assert.AreEqual(true, animal.AllColor.Accessory.IsDirty);
            Assert.AreEqual(true, animal.AllColor.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllColor配饰Door修改", aniMemmory.AllColor.Accessory.Door.Name);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest17()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllColor.Accessory.Door.Name, "AllColor配饰Door");

            // 修改
            AnimalDoor door = new AnimalDoor(60) { Name = "NewAllColor配饰Door" };
            animal.AllColor = new AnimalColor("New颜色", 8, true, AnimalCategory.Empty,
                new AnimalAccessory("NewAllColor配饰", 10, new DateTime(2017, 6, 10), door, AnimalEye.Empty));

            Assert.AreEqual(true, animal.AllColor.Accessory.IsDirty);
            Assert.AreEqual(true, animal.AllColor.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllColor配饰Door", aniMemmory.AllColor.Accessory.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest18()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllWheel.Door.Name, "AllWheelDoor");

            // 修改
            animal.AllWheel.Door.Name = "AllWheelDoor修改";

            Assert.AreEqual(true, animal.AllWheel.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllWheelDoor修改", aniMemmory.AllWheel.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest19()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllWheel.Door.Name, "AllWheelDoor");

            // 修改

            animal.AllWheel.Door = new AnimalDoor(11) {  Name = "NewAllWheelDoor" };

            Assert.AreEqual(true, animal.AllWheel.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllWheelDoor", aniMemmory.AllWheel.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest20()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Door.Name, "MainWheelDoor");

            // 修改
            animal.GetMainWheel().Door.Name = "MainWheelDoor修改";
            Assert.AreEqual(true, animal.GetMainWheel().IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("MainWheelDoor修改", aniMemmory.GetMainWheel().Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest21()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.GetMainWheel().Door.Name, "MainWheelDoor");

            // 修改
            animal.GetMainWheel().Door = new AnimalDoor(12) { Name = "NewMainWheelDoor" };

            Assert.AreEqual(true, animal.GetMainWheel().IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewMainWheelDoor", aniMemmory.GetMainWheel().Door.Name);

            CheckAnimalVersion(animal.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest22()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Door.Name, "AllAnimalBreakDoor");

            // 修改
            animal.AllBreak.Door.Name = "AllAnimalBreakDoor修改";
            Assert.AreEqual(true, animal.AllBreak.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllAnimalBreakDoor修改", aniMemmory.AllBreak.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

            CheckAnimalBreakVersion(aniMemmory.AllBreak.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest23()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Door.Name, "AllAnimalBreakDoor");

            // 修改
            animal.AllBreak.Door = new AnimalDoor(13) { Name = "NewAllAnimalBreakDoor" };

            Assert.AreEqual(true, animal.AllBreak.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllAnimalBreakDoor", aniMemmory.AllBreak.Door.Name);

            CheckAnimalVersion(animal.Id, 2);

            CheckAnimalBreakVersion(aniMemmory.AllBreak.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest24()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllAccessory.Eye.Description, "AllAccessoryAnimalEye");

            // 修改
            animal.AllAccessory.Eye.Description = "AllAccessoryAnimalEye修改";

            Assert.AreEqual(true, animal.AllAccessory.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllAccessoryAnimalEye修改", aniMemmory.AllAccessory.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest25()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllAccessory.Eye.Description, "AllAccessoryAnimalEye");

            // 修改
            AnimalEye eye = new AnimalEye(21)
            {
                Description = "NewAllAccessoryAnimalEye",
                CreateDate = new DateTime(2017, 6, 1)
            };

            animal.AllAccessory =  new AnimalAccessory("NewAllAccessory", 5, new DateTime(2017, 6, 11), 
                AnimalDoor.Empty, eye);

            Assert.AreEqual(true, animal.AllAccessory.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllAccessoryAnimalEye", aniMemmory.AllAccessory.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest26()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllWheel.Eye.Description, "AllWheelDoorEye");

            // 修改
            animal.AllWheel.Eye.Description = "AllWheelDoorEye修改";

            Assert.AreEqual(true, animal.AllWheel.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllWheelDoorEye修改", aniMemmory.AllWheel.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalEyeVersion(aniMemmory.AllWheel.Eye.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest27()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllWheel.Eye.Description, "AllWheelDoorEye");

            // 修改
            AnimalEye eye = new AnimalEye(11)
            {
                Description = "NewAllWheelDoorEye",
                CreateDate = new DateTime(2017, 6, 1)
            };
            animal.AllWheel.Eye = eye;

            Assert.AreEqual(true, animal.AllWheel.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllWheelDoorEye", aniMemmory.AllWheel.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
        }

        [TestMethod]
        public void AnimalComplexTest28()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Eye.Description, "AllAnimalBreakAnimalEye");

            // 修改
            animal.AllBreak.Eye.Description = "AllAnimalBreakAnimalEye修改";

            Assert.AreEqual(true, animal.AllBreak.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("AllAnimalBreakAnimalEye修改", aniMemmory.AllBreak.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalEyeVersion(aniMemmory.AllBreak.Eye.Id, 2);

        }

        [TestMethod]
        public void AnimalComplexTest29()
        {
            var animal = this.Fixture.Get<Animal>() as Animal;
            CheckAnimalVersion(animal.Id, 1);

            Animal aniMemmory = GetAnimal(animal.Id);
            Assert.AreEqual(aniMemmory.AllBreak.Eye.Description, "AllAnimalBreakAnimalEye");

            // 修改
            AnimalEye eye = new AnimalEye(31)
            {
                Description = "NewAllAnimalBreakAnimalEye",
                CreateDate = new DateTime(2017, 6, 1)
            };
            animal.AllBreak.Eye = eye;

            Assert.AreEqual(true, animal.AllBreak.IsDirty);
            Assert.AreEqual(true, animal.IsDirty);

            UpdateAnimal(animal);

            aniMemmory = GetAnimal(animal.Id);

            Assert.AreEqual("NewAllAnimalBreakAnimalEye", aniMemmory.AllBreak.Eye.Description);

            CheckAnimalVersion(animal.Id, 2);
            CheckAnimalBreakVersion(aniMemmory.AllBreak.Id, 2);
            CheckAnimalEyeVersion(aniMemmory.AllBreak.Eye.Id, 1);
        }

        #endregion

        #region 测试工具

        private Animal CreateAnimal()
        {
            AnimalCategory category = InsertAnimalCategory(1, "爬行类", new DateTime(2017, 6, 1));

            Animal animal = new Animal(Guid.NewGuid());
            animal.Category = category;
            animal.Name = "一只动物";
            animal.LiveTime = new DateTime(2017, 6, 1);

            AnimalEye eye1 = new AnimalEye(1)
            {
                Description = "AllWheelDoorEye",
                CreateDate = new DateTime(2017, 6, 1)
            };

            AnimalWheel allWheel = new AnimalWheel(40)
            {
                OrderIndex = 1,
                Description = "the allWheel",
                TheColor = new AnimalColor("主色", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty),
                Category = InsertAnimalCategory(90, "main Wheel动物类", new DateTime(2017, 6, 1)),
                Accessory = new AnimalAccessory("allWheel配饰", 5, new DateTime(2017, 6, 11), AnimalDoor.Empty, AnimalEye.Empty),
                Door = new AnimalDoor(2) { Name = "AllWheelDoor" },
                Eye = eye1
            };
            animal.AllWheel = allWheel;

            AnimalCategory category0 = InsertAnimalCategory(10, "AllColor动物类", new DateTime(2017, 6, 1));

            AnimalDoor door = new AnimalDoor(3) { Name = "AllColor配饰Door" };
            animal.AllColor = new AnimalColor("颜色", 8, true, category0, 
                new AnimalAccessory("AllColor配饰", 10, new DateTime(2017, 6, 10), door, AnimalEye.Empty));

            AnimalDoor door2 = new AnimalDoor(4) { Name = "AllAccessoryDoor" };

            AnimalEye eye2 = new AnimalEye(2)
            {
                Description = "AllAccessoryAnimalEye",
                CreateDate = new DateTime(2017, 6, 1)
            };
            animal.AllAccessory = new AnimalAccessory("AllAccessory", 8, new DateTime(2017, 6, 10), door2, eye2);

            List<AnimalColor> colors = new List<AnimalColor>();

            AnimalCategory category1 = InsertAnimalCategory(11, "红色动物类", new DateTime(2017, 6, 1));
            AnimalCategory category2 = InsertAnimalCategory(12, "蓝色动物类", new DateTime(2017, 6, 1));

            colors.Add(new AnimalColor("红色", 6, true, category1, AnimalAccessory.Empty));
            colors.Add(new AnimalColor("蓝色", 7, true, category2, AnimalAccessory.Empty));

            animal.SetMyColors(colors);

            AnimalWheel mainWheel = new AnimalWheel(30)
            {
                OrderIndex = 1,
                Description = "the main Wheel",
                TheColor = new AnimalColor("主色", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty),
                Category = InsertAnimalCategory(20, "main Wheel动物类", new DateTime(2017, 6, 1)),
                Accessory = new AnimalAccessory("MainWheel配饰", 5, new DateTime(2017, 6, 11), AnimalDoor.Empty, AnimalEye.Empty),
                Door = new AnimalDoor(5){Name = "MainWheelDoor" }
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
                CreateDate = new DateTime(2017, 6, 1),
                Category = InsertAnimalCategory(30, "MyAnimalBreak动物类", new DateTime(2017, 6, 30)),
                Accessory = new AnimalAccessory("MyAnimalBreak配饰", 6, new DateTime(2017, 6, 16), AnimalDoor.Empty, AnimalEye.Empty)
            };

            animal.SetMainBreak(mainBreak);

            AnimalEye eye3 = new AnimalEye(3)
            {
                Description = "AllAnimalBreakAnimalEye",
                CreateDate = new DateTime(2017, 6, 1)
            };

            AnimalBreak allBreak = new AnimalBreak(50)
            {
                Description = "AllAnimalBreak",
                CreateDate = new DateTime(2017, 6, 1),
                Category = InsertAnimalCategory(40, "AllAnimalBreak动物类", new DateTime(2017, 6, 30)),
                Door = new AnimalDoor(6) { Name = "AllAnimalBreakDoor" },
                Eye = eye3
            };

            animal.AllBreak = allBreak;

            return animal;
        }

        private void UpdateAnimal(Animal animal)
        {
            this.BeginTransaction();

            var repository = Repository.Create<IAnimalRepository>();
            repository.Update(animal);

            this.Commit();
        }

        private Animal GetAnimal(Guid id)
        {
            var repository = Repository.Create<IAnimalRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private AnimalCategory InsertAnimalCategory(int id, string name, DateTime date)
        {
            this.BeginTransaction();

            AnimalCategory category = new AnimalCategory(id);

            category.Name = name;
            category.CreateDate = date;

            var repository = Repository.Create<IAnimalCategoryRepository>();
            repository.Add(category);

            this.Commit();

            return category;
        }

        private void UpdateAnimalCategory(int id, string name, DateTime date)
        {
            this.BeginTransaction();

            AnimalCategory mycategory = GetAnimalCategory(id, QueryLevel.Single);
            mycategory.Name = name;
            mycategory.CreateDate = date;

            var repository = Repository.Create<IAnimalCategoryRepository>();
            repository.Update(mycategory);

            this.Commit();
        }

        private AnimalCategory GetAnimalCategory(int id, QueryLevel level)
        {
            var repository = Repository.Create<IAnimalCategoryRepository>();
            return repository.Find(id, level);
        }

        private void CheckAnimalVersion(Guid id, int dataVersion)
        {
            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from Animal where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        private void CheckAnimalCategoryVersion(int id, int dataVersion)
        {
            DataPortal.Direct<AnimalCategory>((conn) =>
            {
                var data = conn.QuerySingle("select * from AnimalCategory where id=@id", new { Id = id });
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

        private void DeleteAnimalCategory(AnimalCategory category)
        {
            this.BeginTransaction();

            var repository = Repository.Create<IAnimalCategoryRepository>();
            repository.Delete(category);

            this.Commit();
        }

        private void CheckAnimalEyeVersion(long id, int dataVersion)
        {
            DataPortal.Direct<Animal>((conn) =>
            {
                var data = conn.QuerySingle("select * from AnimalEye where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        #endregion
    }
}
