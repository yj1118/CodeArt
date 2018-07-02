using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DomainDriven;
using CodeArt.TestTools;
using CodeArt.Concurrent;

namespace CodeArt.DomainDrivenTest
{
    [TestClass]
    public class DomainObjectValidatorTest : UnitTest
    {
        [TestMethod]
        public void ValidatorsCount()
        {
            var category = new BookCategory(1);
            var count = category.Validators.Count();
            Assert.AreEqual(2, count);

        }

        [ObjectValidator(typeof(BookCategoryValidator1),typeof(BookCategoryValidator2))]
        public class BookCategory : AggregateRoot<BookCategory, int>
        {
            #region 静态成员

            public static readonly DomainProperty NameProperty = null;

            /// <summary>
            /// 空对象
            /// </summary>
            public static readonly BookCategory Empty;

            static BookCategory()
            {
                NameProperty = DomainProperty.Register<string, BookCategory>("Name", string.Empty);
                Empty = new BookCategoryEmpty();
            }

            /// <summary>
            /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
            /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
            /// </summary>
            private class BookCategoryEmpty : BookCategory
            {
                public BookCategoryEmpty()
                    : base(0)
                {
                }

                public override bool IsEmpty()
                {
                    return true;
                }
            }

            #endregion

            [PropertyRepository]
            [StringLength(1, 20)]
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


            public BookCategory(int id)
                : base(id)
            {

            }
        }

        [SafeAccess]
        public class BookCategoryValidator1 : ObjectValidator<BookCategory>
        {
            protected override void Validate(BookCategory obj, ValidationResult result)
            {

            }
        }

        [SafeAccess]
        public class BookCategoryValidator2 : ObjectValidator<BookCategory>
        {
            protected override void Validate(BookCategory obj, ValidationResult result)
            {

            }
        }

    }
}
