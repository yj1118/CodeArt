using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(IAnimalRepository))]
    public class Animal : AggregateRoot<Animal, Guid>
    {
        private static readonly DomainProperty CategoryProperty = DomainProperty.Register<AnimalCategory, Animal>("Category", AnimalCategory.Empty);

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

        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Animal>("Name");

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

        #region 生存时间

        [PropertyRepository()]
        private static readonly DomainProperty LiveTimeProperty = DomainProperty.Register<Emptyable<DateTime>, Animal>("LiveTime");

        public Emptyable<DateTime> LiveTime
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(LiveTimeProperty);
            }
            set
            {
                SetValue(LiveTimeProperty, value);
            }
        }

        #endregion

        #region 值对象

        private static readonly DomainProperty AllColorProperty = DomainProperty.Register<AnimalColor, Animal>("AllColor", AnimalColor.Empty);

        [PropertyRepository()]
        public AnimalColor AllColor
        {
            get
            {
                return GetValue<AnimalColor>(AllColorProperty);
            }
            set
            {
                SetValue(AllColorProperty, value);
            }
        }

        private static readonly DomainProperty AllAccessoryProperty = DomainProperty.Register<AnimalAccessory, Animal>("AllAccessory", AnimalAccessory.Empty);

        [PropertyRepository()]
        public AnimalAccessory AllAccessory
        {
            get
            {
                return GetValue<AnimalAccessory>(AllAccessoryProperty);
            }
            set
            {
                SetValue(AllAccessoryProperty, value);
            }
        }


        #endregion



        private static readonly DomainProperty AllWheelProperty = DomainProperty.Register<AnimalWheel, Animal>("AllWheel", AnimalWheel.Empty);

        [PropertyRepository()]
        public AnimalWheel AllWheel
        {
            get
            {
                return GetValue<AnimalWheel>(AllWheelProperty);
            }
            set
            {
                SetValue(AllWheelProperty, value);
            }
        }

        private static readonly DomainProperty AllBreakProperty = DomainProperty.Register<AnimalBreak, Animal>("AllBreak", AnimalBreak.Empty);

        [PropertyRepository(Lazy = true)]
        public AnimalBreak AllBreak
        {
            get
            {
                return GetValue<AnimalBreak>(AllBreakProperty);
            }
            set
            {
                SetValue(AllBreakProperty, value);
            }
        }

        #region 空对象

        private class AnimalEmpty : Animal
        {
            public AnimalEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly Animal Empty = new AnimalEmpty();

        #endregion

        [ConstructorRepository()]
        public Animal(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}