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

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarRepository))]
    public class Car : AggregateRoot<Car, Guid>
    {
        #region 品牌

        private static readonly DomainProperty BrandProperty = DomainProperty.Register<CarBrand, Car>("Brand", CarBrand.Empty);

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

        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Car>("Name");

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

        #region 新旧标识

        private static readonly DomainProperty IsNewCarProperty = DomainProperty.Register<bool, Car>("IsNewCar");

        [PropertyRepository()]
        public bool IsNewCar
        {
            get
            {
                return GetValue<bool>(IsNewCarProperty);
            }
            set
            {
                SetValue(IsNewCarProperty, value);
            }
        }

        #endregion

        #region 基本值的集合1

        private static readonly DomainProperty LightCountsProperty = DomainProperty.RegisterCollection<int, Car>("LightCounts");

        [PropertyRepository()]
        public DomainCollection<int> LightCounts
        {
            get
            {
                return GetValue<DomainCollection<int>>(LightCountsProperty);
            }
            set
            {
                SetValue(LightCountsProperty, value);
            }
        }

        #endregion

        #region 基本值的集合2

        private static readonly DomainProperty ErrorMessagesProperty = DomainProperty.RegisterCollection<string, Car>("ErrorMessages");

        private DomainCollection<string> _ErrorMessages
        {
            get
            {
                return GetValue<DomainCollection<string>>(ErrorMessagesProperty);
            }
            set
            {
                SetValue(ErrorMessagesProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<string> ErrorMessages
        {
            get
            {
                return _ErrorMessages;
            }
            set
            {
                _ErrorMessages =  new DomainCollection<string>(ErrorMessagesProperty, value);
            }
        }

        #endregion

        #region 基本值的集合3

        private static readonly DomainProperty DeliveryDatesProperty = DomainProperty.RegisterCollection<Emptyable<DateTime>, Car>("DeliveryDates");

        private DomainCollection<Emptyable<DateTime>> _DeliveryDates
        {
            get
            {
                return GetValue<DomainCollection<Emptyable<DateTime>>>(DeliveryDatesProperty);
            }
            set
            {
                SetValue(DeliveryDatesProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<Emptyable<DateTime>> DeliveryDates
        {
            get
            {
                return _DeliveryDates;
            }
            set
            {
                _DeliveryDates = new DomainCollection<Emptyable<DateTime>>(DeliveryDatesProperty, value);
            }
        }

        public void AddDeliveryDate(Emptyable<DateTime> deliveryDate)
        {
            _DeliveryDates.Add(deliveryDate);
        }

        public void RemoveDeliveryDate(Emptyable<DateTime> deliveryDate)
        {
            _DeliveryDates.Remove(deliveryDate);
        }
        #endregion

        #region 简单值对象

        private static readonly DomainProperty AllColorProperty = DomainProperty.Register<WholeColor, Car>("AllColor", WholeColor.Empty);

        [PropertyRepository()]
        public WholeColor AllColor
        {
            get
            {
                return GetValue<WholeColor>(AllColorProperty);
            }
            set
            {
                SetValue(AllColorProperty, value);
            }
        }


        #endregion

        #region 简单值对象的集合

        [PropertyRepository()]
        private static readonly DomainProperty CarAccessoriesProperty = DomainProperty.RegisterCollection<CarAccessory, Car>("CarAccessories");

        private DomainCollection<CarAccessory> _carAccessories
        {
            get
            {
                return GetValue<DomainCollection<CarAccessory>>(CarAccessoriesProperty);
            }
            set
            {
                SetValue(CarAccessoriesProperty, value);
            }
        }

        public IEnumerable<CarAccessory> CarAccessories
        {
            get
            {
                return _carAccessories;
            }
            set
            {
                _carAccessories = new DomainCollection<CarAccessory>(CarAccessoriesProperty, value);
            }
        }

        public void AddCarAccessory(CarAccessory carAccessory)
        {
            _carAccessories.Add(carAccessory);
        }

        public void RemoveCarAccessory(CarAccessory carAccessory)
        {
            _carAccessories.Remove(carAccessory);
        }
        #endregion

        #region 实体对象

        /// <summary>
        /// 引用对象
        /// </summary>

        private static readonly DomainProperty MainWheelProperty = DomainProperty.Register<CarWheel, Car>("MainWheel", CarWheel.Empty);

        [PropertyRepository()]
        public CarWheel MainWheel
        {
            get
            {
                return GetValue<CarWheel>(MainWheelProperty);
            }
            set
            {
                SetValue(MainWheelProperty, value);
            }
        }

        #endregion

        #region 实体对象的集合--非延时加载

        /// <summary>
        /// 引用对象的集合
        /// </summary>
        private static readonly DomainProperty WheelsProperty = DomainProperty.RegisterCollection<CarWheel, Car>("Wheels");

        private DomainCollection<CarWheel> _Wheels
        {
            get
            {
                return GetValue<DomainCollection<CarWheel>>(WheelsProperty);
            }
            set
            {
                SetValue(WheelsProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<CarWheel> Wheels
        {
            get
            {
                return _Wheels;
            }
        }

        public void AddCarWheel(CarWheel wheel)
        {
            _Wheels.Add(wheel);
        }

        public void RemoveCarWheel(int wheelId)
        {
            var wheel = _Wheels.FirstOrDefault((t) => t.Id == wheelId);
            _Wheels.Remove(wheel);
        }

        #endregion

        #region 实体对象的集合--延时加载

        /// <summary>
        /// 引用对象的集合
        /// </summary>
        private static readonly DomainProperty DoorsProperty = DomainProperty.RegisterCollection<CarDoor, Car>("Doors");

        private DomainCollection<CarDoor> _Doors
        {
            get
            {
                return GetValue<DomainCollection<CarDoor>>(DoorsProperty);
            }
            set
            {
                SetValue(DoorsProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public IEnumerable<CarDoor> Doors
        {
            get
            {
                return _Doors;
            }
        }

        public void AddCarDoor(CarDoor door)
        {
            _Doors.Add(door);

        }

        public void RemoveCarDoor(int doorId)
        {
            var door = _Doors.FirstOrDefault((t) => t.Id == doorId);
            _Doors.Remove(door);
        }

        #endregion

        #region 高级实体对象

        /// <summary>
        /// 高级引用对象
        /// </summary>

        private static readonly DomainProperty MainBreakProperty = DomainProperty.Register<CarBreak, Car>("MainBreak", CarBreak.Empty);

        [PropertyRepository(Lazy = true)]
        public CarBreak MainBreak
        {
            get
            {
                return GetValue<CarBreak>(MainBreakProperty);
            }
            set
            {
                SetValue(MainBreakProperty, value);
            }
        }

        #endregion

        #region 高级实体对象的集合

        /// <summary>
        /// 高级引用对象的集合
        /// </summary>
        private static readonly DomainProperty BreaksProperty = DomainProperty.RegisterCollection<CarBreak, Car>("Breaks");

        private DomainCollection<CarBreak> _Breaks
        {
            get
            {
                return GetValue<DomainCollection<CarBreak>>(BreaksProperty);
            }
            set
            {
                SetValue(BreaksProperty, value);
            }
        }

        [PropertyRepository(Lazy = true)]
        public IEnumerable<CarBreak> Breaks
        {
            get
            {
                return _Breaks;
            }
        }

        public void AddCarBreak(CarBreak carbreak)
        {
            _Breaks.Add(carbreak);
        }

        public void RemoveCarBreak(long breakId)
        {
            var carbreak = _Breaks.FirstOrDefault((t) => t.Id == breakId);
            _Breaks.Remove(carbreak);
        }

        #endregion

        #region 远程动态对象

        private static readonly DomainProperty OwnerProperty = DomainProperty.RegisterDynamic<CarUser, Car>("Owner");

        [PropertyRepository]
        public dynamic Owner
        {
            get
            {
                return GetValue<dynamic>(OwnerProperty);
            }
            set
            {
                SetValue(OwnerProperty, value);
            }
        }

        #endregion

        #region 空对象

        private class CarEmpty : Car
        {
            public CarEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Car Empty = new CarEmpty();

        #endregion

        public Car(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Car(Guid id, DomainCollection<CarWheel> wheels)
            : base(id)
        {
            _Wheels = wheels;
            this.OnConstructed();
        }
    }


}
