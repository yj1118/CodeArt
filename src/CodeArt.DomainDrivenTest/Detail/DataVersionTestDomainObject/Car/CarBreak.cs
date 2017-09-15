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
    public class CarBreak : EntityObject<CarBreak, long>
    {
        #region 品牌

        private static readonly DomainProperty BrandProperty = DomainProperty.Register<CarBrand, CarBreak>("Brand", CarBrand.Empty);

        [PropertyRepository()]
        public CarBrand Brand
        {
            get
            {
                return GetValue<CarBrand>(BrandProperty);
            }
            set
            {
                SetValue(BrandProperty, value);
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

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<WholeColor, CarBreak>("TheColor", WholeColor.Empty);

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

        private static readonly DomainProperty AccessoryProperty = DomainProperty.Register<CarAccessory, CarBreak>("Accessory", CarAccessory.Empty);

        [PropertyRepository()]
        public CarAccessory Accessory
        {
            get
            {
                return GetValue<CarAccessory>(AccessoryProperty);
            }
            set
            {
                SetValue(AccessoryProperty, value);
            }
        }

        #region 空对象

        public static readonly CarBreak Empty = new CarBreakEmpty();

        private class CarBreakEmpty : CarBreak
        {
            public CarBreakEmpty()
                : base(0)
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
        public CarBreak(long id)
            : base(id)
        {
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}