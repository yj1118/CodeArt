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
    public class CarDoor : EntityObject<CarDoor, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarDoor>("Name", string.Empty);

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

        #region 

        public static readonly DomainProperty OrderIndexProperty = DomainProperty.Register<byte, CarDoor>("OrderIndex", 0);

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

        #region 颜色

        private static readonly DomainProperty TheColorProperty = DomainProperty.Register<WholeColor, CarDoor>("TheColor", (owner) => WholeColor.Empty);

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

        public static readonly CarDoor Empty = new CarDoorEmpty();

        private class CarDoorEmpty : CarDoor
        {
            public CarDoorEmpty()
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
        public CarDoor(int id)
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