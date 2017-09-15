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
    public class AnimalWheel : EntityObject<AnimalWheel, int>
    {
        #region 

        public static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<byte, AnimalWheel>("OrderIndex");

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

        #region 

        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, AnimalWheel>("Description");

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

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<AnimalColor, AnimalWheel>("TheColor", AnimalColor.Empty);

        [PropertyRepository()]
        [NotEmpty]
        public AnimalColor TheColor
        {
            get
            {
                return GetValue<AnimalColor>(TheColorProperty);
            }
            set
            {
                SetValue(TheColorProperty, value);
            }
        }


        #endregion

        private static readonly DomainProperty CategoryProperty = DomainProperty.Register<AnimalCategory, AnimalWheel>("Category", AnimalCategory.Empty);

        [PropertyRepository()]
        public AnimalCategory Category
        {
            get
            {
                return GetValue<AnimalCategory>(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }

        private static readonly DomainProperty AccessoryProperty = DomainProperty.Register<AnimalAccessory, AnimalWheel>("Accessory", AnimalAccessory.Empty);

        [PropertyRepository()]
        public AnimalAccessory Accessory
        {
            get
            {
                return GetValue<AnimalAccessory>(AccessoryProperty);
            }
            set
            {
                SetValue(AccessoryProperty, value);
            }
        }

        private static readonly DomainProperty DoorProperty = DomainProperty.Register<AnimalDoor, AnimalWheel>("Door", AnimalDoor.Empty);

        [PropertyRepository()]
        public AnimalDoor Door
        {
            get
            {
                return GetValue<AnimalDoor>(DoorProperty);
            }
            set
            {
                SetValue(DoorProperty, value);
            }
        }

        private static readonly DomainProperty EyeProperty = DomainProperty.Register<AnimalEye, AnimalWheel>("Eye", AnimalEye.Empty);

        [PropertyRepository()]
        public AnimalEye Eye
        {
            get
            {
                return GetValue<AnimalEye>(EyeProperty);
            }
            set
            {
                SetValue(EyeProperty, value);
            }
        }

        #region 空对象

        public static readonly AnimalWheel Empty = new AnimalWheelEmpty();

        private class AnimalWheelEmpty : AnimalWheel
        {
            public AnimalWheelEmpty()
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
        public AnimalWheel(int id)
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