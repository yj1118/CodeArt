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
    public class AnimalBreak : EntityObject<AnimalBreak, long>
    {

        #region 描述

        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, AnimalBreak>("Description", string.Empty);

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

        #region 创建时间

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, AnimalBreak>("CreateDate");

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

        private static readonly DomainProperty CategoryProperty = DomainProperty.Register<AnimalCategory, AnimalBreak>("Category", AnimalCategory.Empty);

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

        private static readonly DomainProperty AccessoryProperty = DomainProperty.Register<AnimalAccessory, AnimalBreak>("Accessory", AnimalAccessory.Empty);

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

        private static readonly DomainProperty DoorProperty = DomainProperty.Register<AnimalDoor, AnimalBreak>("Door", AnimalDoor.Empty);

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

        private static readonly DomainProperty EyeProperty = DomainProperty.Register<AnimalEye, AnimalBreak>("Eye", AnimalEye.Empty);

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

        public static readonly AnimalBreak Empty = new AnimalBreakEmpty();

        private class AnimalBreakEmpty : AnimalBreak
        {
            public AnimalBreakEmpty()
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
        public AnimalBreak(long id)
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