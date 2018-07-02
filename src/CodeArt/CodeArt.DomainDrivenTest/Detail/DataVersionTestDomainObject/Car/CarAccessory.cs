using CodeArt.DomainDriven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 车辆配饰
    /// </summary>
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class CarAccessory : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarAccessory>("Name");

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

        private static readonly DomainProperty AccessoryNumProperty = DomainProperty.Register<short, CarAccessory>("AccessoryNum");

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

        private static readonly DomainProperty SetupDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarAccessory>("SetupDate");

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

        private class CarAccessoryEmpty : CarAccessory
        {
            public CarAccessoryEmpty()
                : base(string.Empty, 0, null)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly CarAccessory Empty = new CarAccessoryEmpty();

        #endregion

        [ConstructorRepository]
        public CarAccessory(string name, short accessoryNum, Emptyable<DateTime> setupDate)
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

}
