using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class CarBreak : EntityObjectPro<Car, CarBreak, long>
    {
        #region 所属Car

        public static readonly DomainProperty CarProperty = DomainProperty.Register<Car, CarBreak>("Car", Car.Empty);

        [PropertyRepository()]
        [NotEmpty()]
        public Car Car
        {
            get
            {
                return GetValue<Car>(CarProperty);
            }
            private set
            {
                SetValue(CarProperty, value);
            }
        }

        #endregion

        #region 描述

        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, CarBreak>("Description", string.Empty);

        [PropertyRepository()]
        public string Description
        {
            get
            {
                return GetValue<string>(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        #endregion

        #region 颜色

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<WholeColor, CarBreak>("TheColor", (owner) => WholeColor.Empty);

        [PropertyRepository()]
        [NotEmpty]
        public WholeColor TheColor
        {
            get
            {
                return GetValue<WholeColor>(TheColorProperty);
            }
            set
            {
                SetValue(TheColorProperty, value);
            }
        }


        #endregion

        #region 创建时间

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarBreak>("CreateDate");

        [PropertyRepository()]
        public Emptyable<DateTime> CreateDate
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(CreateDateProperty);
            }
            set
            {
                SetValue(CreateDateProperty, value);
            }
        }

        #endregion

        #region 空对象

        public static readonly CarBreak Empty = new CarBreakEmpty();

        private class CarBreakEmpty : CarBreak
        {
            public CarBreakEmpty()
                : base(0, Car.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        #endregion

        [ConstructorRepository()]
        public CarBreak(long id, Car car)
            : base(id)
        {
            this.Car = car;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }

        protected override Car GetRoot()
        {
            return this.Car;
        }
    }
}