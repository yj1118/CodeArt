using CodeArt.DomainDriven;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeArt.DomainDrivenTest.Detail
{
    [ObjectRepository(typeof(IAnimalRepository))]
    [ObjectValidator()]
    public class AnimalEye : EntityObject<AnimalEye, long>
    {
        #region 描述

        public static readonly DomainProperty DescriptionProperty = DomainProperty.Register<string, AnimalEye>("Description", string.Empty);

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

        private static readonly DomainProperty CreateDateProperty = DomainProperty.Register<Emptyable<DateTime>, AnimalEye>("CreateDate");

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

        public static readonly AnimalEye Empty = new AnimalEyeEmpty();

        private class AnimalEyeEmpty : AnimalEye
        {
            public AnimalEyeEmpty()
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
        public AnimalEye(long id)
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