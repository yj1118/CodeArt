using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarBrandRepository))]
    [ObjectValidator()]
    public class CarBrand : AggregateRoot<CarBrand, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarBrand>("Name");

        [PropertyRepository()]
        [StringLength(1, 100)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 创建时间

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarBrand>("CreateDate");

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

        #region BrandAccessory值对象1

        private static readonly DomainProperty BrandAccessoryProperty = DomainProperty.Register<BrandAccessory, CarBrand>("BrandAccessory", BrandAccessory.Empty);

        [PropertyRepository()]
        public BrandAccessory BrandAccessory
        {
            get
            {
                return GetValue<BrandAccessory>(BrandAccessoryProperty);
            }
            set
            {
                SetValue(BrandAccessoryProperty, value);
            }
        }

        #endregion

        #region BrandAccessory值对象2

        private static readonly DomainProperty OtherAccessoryProperty = DomainProperty.Register<BrandAccessory, CarBrand>("OtherAccessory", BrandAccessory.Empty);

        [PropertyRepository()]
        public BrandAccessory OtherAccessory
        {
            get
            {
                return GetValue<BrandAccessory>(OtherAccessoryProperty);
            }
            set
            {
                SetValue(OtherAccessoryProperty, value);
            }
        }

        #endregion

        #region 值对象的集合1

        [PropertyRepository()]
        private static readonly DomainProperty MyAccessoriesProperty = DomainProperty.RegisterCollection<BrandAccessory, CarBrand>("MyAccessories");

        private DomainCollection<BrandAccessory> _MyAccessories
        {
            get
            {
                return GetValue<DomainCollection<BrandAccessory>>(MyAccessoriesProperty);
            }
            set
            {
                SetValue(MyAccessoriesProperty, value);
            }
        }

        public IEnumerable<BrandAccessory> MyAccessories
        {
            get
            {
                return _MyAccessories;
            }
            set
            {
                _MyAccessories = new DomainCollection<BrandAccessory>(MyAccessoriesProperty, value);
            }
        }

        public void AddMyAccessory(BrandAccessory accessory)
        {
            _MyAccessories.Add(accessory);
        }

        public void RemoveMyAccessory(BrandAccessory accessory)
        {
            _MyAccessories.Remove(accessory);
        }
        #endregion

        #region 值对象的集合2

        [PropertyRepository()]
        private static readonly DomainProperty YouAccessoriesProperty = DomainProperty.RegisterCollection<BrandAccessory, CarBrand>("YouAccessories");

        private DomainCollection<BrandAccessory> _YouAccessories
        {
            get
            {
                return GetValue<DomainCollection<BrandAccessory>>(YouAccessoriesProperty);
            }
            set
            {
                SetValue(YouAccessoriesProperty, value);
            }
        }

        public IEnumerable<BrandAccessory> YouAccessories
        {
            get
            {
                return _YouAccessories;
            }
            set
            {
                _YouAccessories = new DomainCollection<BrandAccessory>(YouAccessoriesProperty, value);
            }
        }

        public void AddYouAccessory(BrandAccessory accessory)
        {
            _YouAccessories.Add(accessory);
        }

        public void RemoveYouAccessory(BrandAccessory accessory)
        {
            _YouAccessories.Remove(accessory);
        }
        #endregion

        #region 引用对象1

        private static readonly DomainProperty OneDoorProperty = DomainProperty.Register<BrandDoor, CarBrand>("OneDoor", BrandDoor.Empty);

        [PropertyRepository()]
        public BrandDoor OneDoor
        {
            get
            {
                return GetValue<BrandDoor>(OneDoorProperty);
            }
            set
            {
                SetValue(OneDoorProperty, value);
            }
        }

        #endregion

        #region 引用对象2

        private static readonly DomainProperty OtherDoorProperty = DomainProperty.Register<BrandDoor, CarBrand>("OtherDoor", BrandDoor.Empty);

        [PropertyRepository()]
        public BrandDoor OtherDoor
        {
            get
            {
                return GetValue<BrandDoor>(OtherDoorProperty);
            }
            set
            {
                SetValue(OtherDoorProperty, value);
            }
        }

        #endregion

        #region  引用对象的集合1

        [PropertyRepository()]
        private static readonly DomainProperty MyDoorsProperty = DomainProperty.RegisterCollection<BrandDoor, CarBrand>("MyDoors");

        private DomainCollection<BrandDoor> _MyDoors
        {
            get
            {
                return GetValue<DomainCollection<BrandDoor>>(MyDoorsProperty);
            }
            set
            {
                SetValue(MyDoorsProperty, value);
            }
        }

        public IEnumerable<BrandDoor> MyDoors
        {
            get
            {
                return _MyDoors;
            }
            set
            {
                _MyDoors = new DomainCollection<BrandDoor>(MyDoorsProperty, value);
            }
        }

        public void AddMyDoor(BrandDoor door)
        {
            _MyDoors.Add(door);
        }

        public void RemoveMyDoor(BrandDoor door)
        {
            _MyDoors.Remove(door);
        }
        #endregion

        #region  引用对象的集合2

        [PropertyRepository()]
        private static readonly DomainProperty OtherDoorsProperty = DomainProperty.RegisterCollection<BrandDoor, CarBrand>("OtherDoors");

        private DomainCollection<BrandDoor> _OtherDoors
        {
            get
            {
                return GetValue<DomainCollection<BrandDoor>>(OtherDoorsProperty);
            }
            set
            {
                SetValue(OtherDoorsProperty, value);
            }
        }

        public IEnumerable<BrandDoor> OtherDoors
        {
            get
            {
                return _OtherDoors;
            }
            set
            {
                _OtherDoors = new DomainCollection<BrandDoor>(OtherDoorsProperty, value);
            }
        }

        public void AddOtherDoor(BrandDoor door)
        {
            _OtherDoors.Add(door);
        }

        public void RemoveOtherDoor(BrandDoor door)
        {
            _OtherDoors.Remove(door);
        }
        #endregion

        #region 空对象

        private class CarBrandEmpty : CarBrand
        {
            public CarBrandEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly CarBrand Empty = new CarBrandEmpty();

        #endregion

        [ConstructorRepository]
        public CarBrand(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }

    /// <summary>
    /// 配饰
    /// </summary>
    [ObjectRepository(typeof(ICarBrandRepository))]
    [ObjectValidator()]
    public class BrandAccessory : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, BrandAccessory>("Name");

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

        #region 配饰数量

        private static readonly DomainProperty AccessoryNumProperty = DomainProperty.Register<short, BrandAccessory>("AccessoryNum");

        [PropertyRepository()]
        public short AccessoryNum
        {
            get
            {
                return GetValue<short>(AccessoryNumProperty);
            }
            private set
            {
                SetValue(AccessoryNumProperty, value);
            }
        }

        #endregion

        #region 装配日期

        private static readonly DomainProperty SetupDateProperty = DomainProperty.Register<Emptyable<DateTime>, BrandAccessory>("SetupDate");

        [PropertyRepository()]
        public Emptyable<DateTime> SetupDate
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(SetupDateProperty);
            }
            private set
            {
                SetValue(SetupDateProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class BrandAccessoryEmpty : BrandAccessory
        {
            public BrandAccessoryEmpty()
                : base(string.Empty, 0, null)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly BrandAccessory Empty = new BrandAccessoryEmpty();

        #endregion

        [ConstructorRepository]
        public BrandAccessory(string name, short accessoryNum, Emptyable<DateTime> setupDate)
        {
            this.Name = name;
            this.AccessoryNum = accessoryNum;
            this.SetupDate = setupDate;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

    [ObjectRepository(typeof(ICarBrandRepository))]
    [ObjectValidator()]
    public class BrandDoor : EntityObject<BrandDoor, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, BrandDoor>("Name", string.Empty);

        [PropertyRepository()]
        [StringLength(1, 100)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        #endregion

        #region 

        public static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<byte, BrandDoor>("OrderIndex", 0);

        [PropertyRepository()]
        public byte OrderIndex
        {
            get
            {
                return GetValue<byte>(OrderIndexProperty);
            }
            set
            {
                SetValue(OrderIndexProperty, value);
            }
        }

        #endregion

        #region 空对象

        public static readonly BrandDoor Empty = new BrandDoorEmpty();

        private class BrandDoorEmpty : BrandDoor
        {
            public BrandDoorEmpty()
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
        public BrandDoor(int id)
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