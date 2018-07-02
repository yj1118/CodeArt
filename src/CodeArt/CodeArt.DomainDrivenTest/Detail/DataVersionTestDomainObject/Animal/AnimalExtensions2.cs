using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;
using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ExtendedClass(typeof(AnimalExtensions), typeof(AnimalExtensions2))]
    public static class AnimalExtensions2
    {
        #region 值对象

        [PropertyRepository()]
        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<AnimalColor, Animal>("TheColor", AnimalColor.Empty);

        public static AnimalColor GetTheColor(this Animal animal)
        {
            return animal.GetValue<AnimalColor>(TheColorProperty);
        }

        public static void SetTheColor(this Animal animal, AnimalColor color)
        {
            animal.SetValue(TheColorProperty, color);
        }


        #endregion

        #region 值对象集合

        [PropertyRepository()]
        private static readonly DomainProperty MyColorsProperty = DomainProperty.RegisterCollection<AnimalColor, Animal>("MyColors");

        public static DomainCollection<AnimalColor> GetMyColors(this Animal animal)
        {
            return animal.GetValue<DomainCollection<AnimalColor>>(MyColorsProperty);
        }

        public static void SetMyColors(this Animal animal, IEnumerable<AnimalColor> items)
        {
            DomainCollection<AnimalColor> dc = new DomainCollection<AnimalColor>(MyColorsProperty, items);

            animal.SetValue(MyColorsProperty, dc);
        }

        #endregion

        #region 实体对象

        /// <summary>
        /// 引用对象
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty MainWheelProperty = DomainProperty.Register<AnimalWheel, Animal>("MainWheel", AnimalWheel.Empty);

        public static AnimalWheel GetMainWheel(this Animal animal)
        {
            return animal.GetValue<AnimalWheel>(MainWheelProperty);
        }

        public static void SetMainWheel(this Animal animal, AnimalWheel wheel)
        {
            animal.SetValue(MainWheelProperty, wheel);
        }

        #endregion

        #region 实体对象的集合

        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty WheelsProperty = DomainProperty.RegisterCollection<AnimalWheel, Animal>("Wheels");

        public static DomainCollection<AnimalWheel> GetWheels(this Animal animal)
        {
            return animal.GetValue<DomainCollection<AnimalWheel>>(WheelsProperty);
        }

        public static void SetWheels(this Animal animal, IEnumerable<AnimalWheel> items)
        {
            DomainCollection<AnimalWheel> dc = new DomainCollection<AnimalWheel>(WheelsProperty, items);

            animal.SetValue(WheelsProperty, dc);
        }

        #endregion

        #region 高级实体对象

        /// <summary>
        /// 高级引用对象
        /// </summary>
        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty MainBreakProperty = DomainProperty.Register<AnimalBreak, Animal>("MainBreak", AnimalBreak.Empty);

        public static AnimalBreak GetMainBreak(this Animal animal)
        {
            return animal.GetValue<AnimalBreak>(MainBreakProperty);
        }

        public static void SetMainBreak(this Animal animal, AnimalBreak Abreak)
        {
            animal.SetValue(MainBreakProperty, Abreak);
        }

        #endregion

        #region 高级实体对象的集合

        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty BreaksProperty = DomainProperty.RegisterCollection<AnimalBreak, Animal>("Breaks");

        public static DomainCollection<AnimalBreak> GetBreaks(this Animal animal)
        {
            return animal.GetValue<DomainCollection<AnimalBreak>>(BreaksProperty);
        }

        public static void SetBreaks(this Animal animal, IEnumerable<AnimalBreak> items)
        {
            DomainCollection<AnimalBreak> dc = new DomainCollection<AnimalBreak>(BreaksProperty, items);

            animal.SetValue(BreaksProperty, dc);
        }


        //public void AddCarBreak(CarBreak carbreak)
        //{
        //    _Breaks.Add(carbreak);

        //    var repository = Repository.Create<ICarRepository>();
        //    repository.AddEntityPro(carbreak);
        //}

        //public void RemoveCarBreak(long breakId)
        //{
        //    var carbreak = _Breaks.FirstOrDefault((t) => t.Id == breakId);
        //    _Breaks.Remove(carbreak);

        //    var repository = Repository.Create<ICarRepository>();
        //    repository.DeleteEntityPro(carbreak);
        //}

        //public void UpdateCarBreak(CarBreak carbreak)
        //{
        //    var repository = Repository.Create<ICarRepository>();
        //    repository.UpdateEntityPro(carbreak);
        //}

        #endregion

    }
}