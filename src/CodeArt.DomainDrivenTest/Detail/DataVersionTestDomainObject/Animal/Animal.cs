using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(IAnimalRepository), NoCache = true)]
    public class Animal : AggregateRoot<Animal, Guid>
    {
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

        private static readonly Animal Empty = new AnimalEmpty();

        #endregion

        [ConstructorRepository()]
        public Animal(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}