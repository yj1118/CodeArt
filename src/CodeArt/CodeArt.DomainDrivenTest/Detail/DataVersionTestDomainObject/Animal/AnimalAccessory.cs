using CodeArt.DomainDriven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.DomainDrivenTest.Detail
{
    /// <summary>
    /// 配饰
    /// </summary>
    [ObjectRepository(typeof(IAnimalCategoryRepository))]
    [ObjectValidator()]
    public class AnimalAccessory : ValueObject
    {
        #region 名称

        public static readonly DomainProperty NameProperty = DomainProperty.Register<string, AnimalAccessory>("Name");

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

        private static readonly DomainProperty AccessoryNumProperty = DomainProperty.Register<short, AnimalAccessory>("AccessoryNum");

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


        private static readonly DomainProperty SetupDateProperty = DomainProperty.Register<Emptyable<DateTime>, AnimalAccessory>("SetupDate");

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

        private static readonly DomainProperty DoorProperty = DomainProperty.Register<AnimalDoor, AnimalAccessory>("Door", AnimalDoor.Empty);

        [PropertyRepository()]
        public AnimalDoor Door
        {
            get
            {
                return GetValue<AnimalDoor>(DoorProperty);
            }
            private set
            {
                SetValue(DoorProperty, value);
            }
        }

        private static readonly DomainProperty EyeProperty = DomainProperty.Register<AnimalEye, AnimalAccessory>("Eye", AnimalEye.Empty);

        [PropertyRepository(Lazy = true)]
        public AnimalEye Eye
        {
            get
            {
                return GetValue<AnimalEye>(EyeProperty);
            }
            private set
            {
                SetValue(EyeProperty, value);
            }
        }

        #region 空对象

        private class AnimalAccessoryEmpty : AnimalAccessory
        {
            public AnimalAccessoryEmpty()
                : base(string.Empty, 0, null, AnimalDoor.Empty, AnimalEye.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly AnimalAccessory Empty = new AnimalAccessoryEmpty();

        #endregion

        [ConstructorRepository]
        public AnimalAccessory(string name, short accessoryNum, Emptyable<DateTime> setupDate, AnimalDoor door, AnimalEye eye)
        {
            this.Name = name;
            this.AccessoryNum = accessoryNum;
            this.SetupDate = setupDate;
            this.Door = door;
            this.Eye = eye;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }

}
