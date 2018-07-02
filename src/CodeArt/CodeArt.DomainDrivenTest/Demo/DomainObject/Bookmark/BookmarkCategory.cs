using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 书签的分类
    /// </summary>
    [ObjectRepository(typeof(IBookRepository), SnapshotLifespan = 24)]
    [ObjectValidator()]
    public class BookmarkCategory : AggregateRoot<BookmarkCategory, int>
    {
        #region 静态成员

        public static readonly DomainProperty NameProperty = null;

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly BookmarkCategory Empty;

        static BookmarkCategory()
        {
            NameProperty = DomainProperty.Register<string, BookmarkCategory>("Name");
            Empty = new BookmarkCategoryEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookmarkCategoryEmpty : BookmarkCategory
        {
            public BookmarkCategoryEmpty()
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

        [PropertyRepository()]
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


        public BookmarkCategory(int id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}
