using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(IAnimalCategoryRepository))]
    [ObjectValidator()]
    public class AnimalCategory : AggregateRoot<AnimalCategory, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, AnimalCategory>("Name");

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

        #region 创建时间

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, AnimalCategory>("CreateDate");

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

        #region 空对象

        private class AnimalCategoryEmpty : AnimalCategory
        {
            public AnimalCategoryEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly AnimalCategory Empty = new AnimalCategoryEmpty();

        #endregion

        [ConstructorRepository]
        public AnimalCategory(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }

}