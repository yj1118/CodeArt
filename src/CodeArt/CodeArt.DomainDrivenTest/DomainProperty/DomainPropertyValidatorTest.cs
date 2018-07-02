using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DomainDriven;
using CodeArt.TestTools;

namespace CodeArt.DomainDrivenTest
{
    [TestClass]
    public class DomainPropertyValidatorTest : UnitTest
    {
        [TestMethod]
        public void ValidatorsCount()
        {
            var count = BookCategory.NameProperty.Validators.Count();
            Assert.AreEqual(1, count);
        }

        public class BookCategory : AggregateRoot<BookCategory, int>
        {
            #region 静态成员

            [PropertyRepository]
            public static readonly DomainProperty NameProperty = null;

            /// <summary>
            /// 空对象
            /// </summary>
            public static readonly BookCategory Empty;

            static BookCategory()
            {
                NameProperty = DomainProperty.Register<string, BookCategory>("Name");
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
    }
}
