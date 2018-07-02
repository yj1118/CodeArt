using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Detail
{
    [DerivedClass(typeof(Dog), "{12BBA751-193A-4eab-9AFA-4FD70C115FDA}")]
    [ObjectRepository(typeof(IDogRepository))]
    public class Dog : Animal
    {
        private static readonly DomainProperty NickNameProperty = DomainProperty.Register<string, Dog>("NickName", string.Empty);

        [PropertyRepository()]
        [StringLength(Min = 1, Max = 100)]
        public string NickName
        {
            get
            {
                return GetValue<string>(NickNameProperty);
            }
            set
            {
                SetValue(NickNameProperty, value);
            }
        }

        private static readonly DomainProperty AgeProperty = DomainProperty.Register<short, Dog>("Age", 0);

        [PropertyRepository()]
        public short Age
        {
            get
            {
                return GetValue<short>(AgeProperty);
            }
            set
            {
                SetValue(AgeProperty, value);
            }
        }

        private static readonly DomainProperty IsGoodProperty = DomainProperty.Register<bool, Dog>("IsGood");

        [PropertyRepository()]
        public bool IsGood
        {
            get
            {
                return GetValue<bool>(IsGoodProperty);
            }
            set
            {
                SetValue(IsGoodProperty, value);
            }
        }


        #region 空对象

        private class DogEmpty : Dog
        {
            public DogEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly new Dog Empty = new DogEmpty();

        #endregion

        [ConstructorRepository()]
        public Dog(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

    }
}
