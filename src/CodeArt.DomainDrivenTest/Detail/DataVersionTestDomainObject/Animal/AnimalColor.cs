using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(IAnimalRepository))]
    [ObjectValidator()]
    public class AnimalColor : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, AnimalColor>("Name");

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

        private static readonly DomainProperty ColorNumProperty = DomainProperty.Register<int, AnimalColor>("ColorNum");

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

        private static readonly DomainProperty IsPaintedProperty = DomainProperty.Register<bool, AnimalColor>("IsPainted");

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

        private static readonly DomainProperty CategoryProperty = DomainProperty.Register<AnimalCategory, AnimalColor>("Category", AnimalCategory.Empty);

        [PropertyRepository()]
        public AnimalCategory Category
        {
            get
            {
                return GetValue<AnimalCategory>(CategoryProperty);
            }
            private set
            {
                SetValue(CategoryProperty, value);
            }
        }

        private static readonly DomainProperty AccessoryProperty = DomainProperty.Register<AnimalAccessory, AnimalColor>("Accessory", AnimalAccessory.Empty);

        [PropertyRepository()]
        public AnimalAccessory Accessory
        {
            get
            {
                return GetValue<AnimalAccessory>(AccessoryProperty);
            }
            private set
            {
                SetValue(AccessoryProperty, value);
            }
        }

        #region 空对象

        private class AnimalColorEmpty : AnimalColor
        {
            public AnimalColorEmpty()
                : base(string.Empty, 0, false, AnimalCategory.Empty, AnimalAccessory.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly AnimalColor Empty = new AnimalColorEmpty();

        #endregion

        [ConstructorRepository]
        public AnimalColor(string name, int colorNum, bool isPainted, AnimalCategory category, AnimalAccessory accessory)
        {
            this.Name = name;
            this.ColorNum = colorNum;
            this.IsPainted = isPainted;
            this.Category = category;
            this.Accessory = accessory;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}