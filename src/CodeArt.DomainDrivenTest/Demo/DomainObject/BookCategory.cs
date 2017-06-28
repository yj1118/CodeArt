using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 书的分类
    /// </summary>
    [ObjectRepository(typeof(IBookCategoryRepository), SnapshotLifespan = 365)]
    [ObjectValidator()]
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
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        #endregion

        [PropertyRepository(Snapshot =true)]
        [StringLength(1, 10)]
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

        [ConstructorRepository]
        public BookCategory(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}