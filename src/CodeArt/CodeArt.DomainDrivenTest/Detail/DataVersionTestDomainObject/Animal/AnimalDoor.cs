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
    public class AnimalDoor : EntityObject<AnimalDoor, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, AnimalDoor>("Name", string.Empty);

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

        #region 空对象

        public static readonly AnimalDoor Empty = new AnimalDoorEmpty();

        private class AnimalDoorEmpty : AnimalDoor
        {
            public AnimalDoorEmpty()
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
        public AnimalDoor(int id)
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