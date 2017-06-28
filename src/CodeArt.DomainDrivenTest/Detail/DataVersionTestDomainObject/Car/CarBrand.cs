using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(ICarBrandRepository), NoCache = true)]
    [ObjectValidator()]
    public class CarBrand : AggregateRoot<CarBrand, int>
    {
        #region 名称

        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, CarBrand>("Name");

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

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, CarBrand>("CreateDate");

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

        private class CarBrandEmpty : CarBrand
        {
            public CarBrandEmpty()
                : base(0)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly CarBrand Empty = new CarBrandEmpty();

        #endregion

        [ConstructorRepository]
        public CarBrand(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}