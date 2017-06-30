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

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<AnimalColor, AnimalWheel>("TheColor", (owner) => AnimalColor.Empty);

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