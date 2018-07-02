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
    public class DataVersionTest6 : DomainStage
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

            GoldenDog gd = CreateGoldenDog();

            var repository = Repository.Create<IGoldenDogRepository>();
            repository.Add(gd);

            this.Commit();

            this.Fixture.Add(gd);
        }

        #region 对象继承的测试

        [TestMethod]
        public void AnimalInherited2Test1()
        {
            var dog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(dog.Id, 1);

            GoldenDog dogMemmory = GetGoldenDog(dog.Id);

            Assert.AreEqual(dogMemmory.LegCounts.Count, 2);

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
        public void AnimalInherited2Test2()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.Age = 18;
            UpdateGoldenDog(goldenDog);

            Dog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.Age, 18);
            CheckGoldenDogVersion(goldenDog.Id, 2);

        }

        [TestMethod]
        public void AnimalInherited2Test3()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.SetSignature("金毛狗的签名");
            UpdateGoldenDog(goldenDog);

            Dog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GetSignature(), "金毛狗的签名");
            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test4()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.LegCounts.Add(3);

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.LegCounts.Count, 3);
            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test5()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.LegColor = new AnimalColor("Leg颜色修改", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty);

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.LegColor.Name, "Leg颜色修改");
            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test6()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.AddHeadColor(new AnimalColor("绿色", 10, true, AnimalCategory.Empty, AnimalAccessory.Empty));

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.HeadColors.Count(), 3);
            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test7()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            List<AnimalColor> colors = new List<AnimalColor>()
            {
                new AnimalColor("绿色", 10, true, AnimalCategory.Empty, AnimalAccessory.Empty),
            };

            goldenDog.HeadColors = colors;

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.HeadColors.Count(), 1);
            Assert.AreEqual(dogMemmory.HeadColors.ElementAt(0).Name, "绿色");

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test8()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.GoldenWheel.Description = "modify the GoldenWheel";

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenWheel.Description, "modify the GoldenWheel");

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test9()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.GoldenWheel = new AnimalWheel(101)
            {
                OrderIndex = 1,
                Description = "other GoldenWheel",
                TheColor = new AnimalColor("Golden2", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenWheel.Description, "other GoldenWheel");

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test10()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.GoldenBreak.Description = "Modify GoldenBreak";

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenBreak.Description, "Modify GoldenBreak");

            CheckGoldenDogVersion(goldenDog.Id, 2);

            CheckAnimalBreakVersion(dogMemmory.GoldenBreak.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test11()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.GoldenBreak.Description = "Modify GoldenBreak";

            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenBreak.Description, "Modify GoldenBreak");

            CheckGoldenDogVersion(goldenDog.Id, 2);

            CheckAnimalBreakVersion(dogMemmory.GoldenBreak.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test12()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改

            goldenDog.GoldenBreak = new AnimalBreak(51)
            {
                Description = "other GoldenBreak",
                CreateDate = new DateTime(2017, 6, 2)
            };

            this.BeginTransaction();

            var repository = Repository.Create<IGoldenDogRepository>();
            repository.Update(goldenDog);

            this.Commit();


            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenBreak.Description, "other GoldenBreak");

            CheckGoldenDogVersion(goldenDog.Id, 2);

            CheckAnimalBreakVersion(dogMemmory.GoldenBreak.Id, 1);
        }

        [TestMethod]
        public void AnimalInherited2Test13()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            goldenDog.GoldenWheels.ElementAt(0).Description = "modify theGoldenWheel1";
            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenWheels.ElementAt(0).Description, "modify theGoldenWheel1");

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test14()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改
            AnimalWheel wheel3 = new AnimalWheel(42)
            {
                OrderIndex = 13,
                Description = "theGoldenWheel3",
                TheColor = new AnimalColor("绿色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            goldenDog.AddGoldenWheel(wheel3);
            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenWheels.Count(), 3);

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test15()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);

            // 修改

            goldenDog.RemoveGoldenWheel(40);
            UpdateGoldenDog(goldenDog);

            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);

            Assert.AreEqual(dogMemmory.GoldenWheels.Count(), 1);

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test16()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);
            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);
            Assert.AreEqual(dogMemmory.GoldenBreaks.Count(), 2);

            // 修改
            AnimalBreak breaker= goldenDog.GoldenBreaks.ElementAt(0);
            breaker.Description = "modify goldenDogBreak1";

            UpdateGoldenDog(goldenDog);

            dogMemmory = GetGoldenDog(goldenDog.Id);

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test17()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);
            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);
            Assert.AreEqual(dogMemmory.GoldenBreaks.Count(), 2);

            // 修改
            AnimalBreak breaker = goldenDog.GoldenBreaks.ElementAt(0);

            goldenDog.RemoveGoldenBreak(breaker.Id);

            UpdateGoldenDog(goldenDog);

            dogMemmory = GetGoldenDog(goldenDog.Id);
            Assert.AreEqual(dogMemmory.GoldenBreaks.Count(), 1);

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        [TestMethod]
        public void AnimalInherited2Test18()
        {
            var goldenDog = this.Fixture.Get<GoldenDog>() as GoldenDog;

            CheckGoldenDogVersion(goldenDog.Id, 1);
            GoldenDog dogMemmory = GetGoldenDog(goldenDog.Id);
            Assert.AreEqual(dogMemmory.GoldenBreaks.Count(), 2);

            // 修改
            AnimalBreak breaker = new AnimalBreak(23)
            {
                Description = "goldenDogBreak3",
                CreateDate = new DateTime(2017, 6, 3)
            };

            goldenDog.AddGoldenBreak(breaker);

            UpdateGoldenDog(goldenDog);

            dogMemmory = GetGoldenDog(goldenDog.Id);
            Assert.AreEqual(dogMemmory.GoldenBreaks.Count(), 3);

            CheckGoldenDogVersion(goldenDog.Id, 2);
        }

        #endregion

        #region 测试工具

        private GoldenDog CreateGoldenDog()
        {
            GoldenDog goldenDog = new GoldenDog(Guid.NewGuid());

            goldenDog.LegCounts.Add(1);
            goldenDog.LegCounts.Add(2);

            goldenDog.LegColor = new AnimalColor("Leg颜色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty);

            List<AnimalColor> colors = new List<AnimalColor>()
            {
                new AnimalColor("蓝色", 8, true, AnimalCategory.Empty, AnimalAccessory.Empty),
                new AnimalColor("红色", 9, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            goldenDog.HeadColors = colors;

            goldenDog.GoldenWheel = new AnimalWheel(100)
            {
                OrderIndex = 1,
                Description = "the GoldenWheel",
                TheColor = new AnimalColor("Golden", 5, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            goldenDog.GoldenBreak = new AnimalBreak(50)
            {
                Description = "GoldenBreak",
                CreateDate = new DateTime(2017, 6, 1)
            };

            AnimalWheel wheel1 = new AnimalWheel(40)
            {
                OrderIndex = 11,
                Description = "theGoldenWheel1",
                TheColor = new AnimalColor("红色", 6, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            AnimalWheel wheel2 = new AnimalWheel(41)
            {
                OrderIndex = 12,
                Description = "theGoldenWheel2",
                TheColor = new AnimalColor("蓝色", 7, true, AnimalCategory.Empty, AnimalAccessory.Empty)
            };

            goldenDog.AddGoldenWheel(wheel1);
            goldenDog.AddGoldenWheel(wheel2);

            AnimalBreak breaker1 = new AnimalBreak(21)
            {
                Description = "goldenDogBreak1",
                CreateDate = new DateTime(2017, 6, 1)
            };

            AnimalBreak breaker2 = new AnimalBreak(22)
            {
                Description = "goldenDogBreak2",
                CreateDate = new DateTime(2017, 6, 2)
            };

            goldenDog.AddGoldenBreak(breaker1);
            goldenDog.AddGoldenBreak(breaker2);

            FillDog(goldenDog);
            FillAnimal(goldenDog); 

            return goldenDog;
        }

        private void FillDog(Dog dog)
        {
            dog.NickName = "MyDog";
            dog.Age = 20;
            dog.IsGood = true;
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

        private void UpdateGoldenDog(GoldenDog gd)
        {
            this.BeginTransaction();

            var repository = Repository.Create<IGoldenDogRepository>();

            repository.Update(gd);

            this.Commit();
        }

        private GoldenDog GetGoldenDog(Guid id)
        {
            var repository = Repository.Create<IGoldenDogRepository>();
            return repository.Find(id, QueryLevel.None);
        }

        private void CheckGoldenDogVersion(Guid id, int dataVersion)
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

        private void CheckAnimalBreakVersion(long id, int dataVersion)
        {
            DataPortal.Direct<GoldenDog>((conn) =>
            {
                var data = conn.QuerySingle("select * from AnimalBreak where id=@id", new { Id = id });
                Assert.AreEqual(data.DataVersion, dataVersion);
            });
        }

        #endregion
    }
}
