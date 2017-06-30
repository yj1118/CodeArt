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
        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<AnimalColor, Animal>("TheColor", (owner) => AnimalColor.Empty);

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

        private static readonly DomainProperty MainWheelProperty = DomainProperty.Register<CarWheel, Animal>("MainWheel", CarWheel.Empty);

        public static CarWheel GetMainWheel(this Animal animal)
        {
            return animal.GetValue<CarWheel>(MainWheelProperty);
        }

        public static void SetMainWheel(this Animal animal, CarWheel wheel)
        {
            animal.SetValue(MainWheelProperty, wheel);
        }

        #endregion

    }
}