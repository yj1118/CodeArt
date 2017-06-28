using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarRepository))]
    [ObjectValidator()]
    public class CarWheel : EntityObject<CarWheel, int>
    {
        #region 

        public static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<byte, CarWheel>("OrderIndex");

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

        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, CarWheel>("Description");

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

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<WholeColor, CarWheel>("TheColor", (owner) => WholeColor.Empty);

        [PropertyRepository()]
        [NotEmpty]
        public WholeColor TheColor
        {
            get
            {
                return GetValue<WholeColor>(TheColorProperty);
            }
            set
            {
                SetValue(TheColorProperty, value);
            }
        }


        #endregion

        #region 空对象

        public static readonly CarWheel Empty = new CarWheelEmpty();

        private class CarWheelEmpty : CarWheel
        {
            public CarWheelEmpty()
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
        public CarWheel(int id)
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