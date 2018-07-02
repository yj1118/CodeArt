using CodeArt.DomainDriven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 全车颜色
    /// </summary>
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class WholeColor : ValueObject
    {
        #region 品牌

        private static readonly DomainProperty BrandProperty = DomainProperty.Register<CarBrand, WholeColor>("Brand", CarBrand.Empty);

        [PropertyRepository()]
        public CarBrand Brand
        {
            get
            {
                return GetValue<CarBrand>(BrandProperty);
            }
            private set
            {
                SetValue(BrandProperty, value);
            }
        }

        #endregion

        private static readonly DomainProperty AccessoryProperty = DomainProperty.Register<CarAccessory, WholeColor>("Accessory", CarAccessory.Empty);

        [PropertyRepository()]
        public CarAccessory Accessory
        {
            get
            {
                return GetValue<CarAccessory>(AccessoryProperty);
            }
            private set
            {
                SetValue(AccessoryProperty, value);
            }
        }


        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, WholeColor>("Name");

        [PropertyRepository()]
        [StringLength(1, 150)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 颜色数量

        private static readonly DomainProperty ColorNumProperty = DomainProperty.Register<int, WholeColor>("ColorNum");

        [PropertyRepository()]
        [IntRange(1, 100)]
        public int ColorNum
        {
            get
            {
                return GetValue<int>(ColorNumProperty);
            }
            private set
            {
                SetValue(ColorNumProperty, value);
            }
        }

        #endregion

        #region 已涂标识

        private static readonly DomainProperty IsPaintedProperty = DomainProperty.Register<bool, WholeColor>("IsPainted");

        [PropertyRepository()]
        public bool IsPainted
        {
            get
            {
                return GetValue<bool>(IsPaintedProperty);
            }
            private set
            {
                SetValue(IsPaintedProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class WholeColorEmpty : WholeColor
        {
            public WholeColorEmpty()
                : base(string.Empty, 0, false)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly WholeColor Empty = new WholeColorEmpty();

        #endregion

        [ConstructorRepository]
        public WholeColor(string name, int colorNum, bool isPainted, CarBrand brand = null, CarAccessory accessory = null)
        {
            this.Name = name;
            this.ColorNum = colorNum;
            this.IsPainted = isPainted;

            if (brand == null)
            {
                this.Brand = CarBrand.Empty;
            }
            else
            {
                this.Brand = brand;
            }

            if (accessory == null)
            {
                this.Accessory = CarAccessory.Empty;
            }
            else
            {
                this.Accessory = accessory;
            }


            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
