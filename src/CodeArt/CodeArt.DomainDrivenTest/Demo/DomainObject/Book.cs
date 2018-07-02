using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    [ObjectRepository(typeof(IBookRepository), SnapshotLifespan = 30)]
    public class Book : AggregateRoot<Book, Guid>
    {
        #region 书名


        private static readonly DomainProperty NameProperty = DomainProperty.Register<string, Book>("Name");


        /// <summary>
        /// 书名
        /// </summary>
        [PropertyRepository(Snapshot = true)]
        [StringLength(1, 100)]
        [PropertySet("SetName")]
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

        protected virtual void SetName(string value)
        {
            this.Name = value;
        }


        #endregion

        #region 分类

        private static readonly DomainProperty CategoryProperty = DomainProperty.Register<BookCategory, Book>("Category", BookCategory.Empty);

        [PropertyRepository(Lazy = true)]
        [NotEmpty]
        public BookCategory Category
        {
            get
            {
                return GetValue<BookCategory>(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }

        #endregion

        #region 封面

        /// <summary>
        /// 值对象
        /// </summary>
        [PropertyRepository(Snapshot = true)]
        [NotEmpty]
        private static readonly DomainProperty CoverProperty = DomainProperty.Register<BookCover, Book>("Cover", BookCover.Empty);

        public BookCover Cover
        {
            get
            {
                return GetValue<BookCover>(CoverProperty);
            }
            set
            {
                SetValue(CoverProperty, value);
            }
        }

        #endregion

        #region 主要标签

        /// <summary>
        /// 引用对象
        /// </summary>

        private static readonly DomainProperty MainBookmarkProperty = DomainProperty.Register<Bookmark, Book>("MainBookmark", Bookmark.Empty);

        [PropertyRepository(Lazy = true)]
        public Bookmark MainBookmark
        {
            get
            {
                return GetValue<Bookmark>(MainBookmarkProperty);
            }
            set
            {
                SetValue(MainBookmarkProperty, value);
            }
        }

        #endregion

        #region 更多标签

        /// <summary>
        /// 引用对象的集合
        /// </summary>
        [PropertyRepository(Lazy = true, Snapshot = true)]
        private static readonly DomainProperty BookmarksProperty = DomainProperty.RegisterCollection<Bookmark, Book>("Bookmarks");

        private DomainCollection<Bookmark> _Bookmarks
        {
            get
            {
                return GetValue<DomainCollection<Bookmark>>(BookmarksProperty);
            }
            set
            {
                SetValue(BookmarksProperty, value);
            }
        }

        public IEnumerable<Bookmark> Bookmarks
        {
            get
            {
                return _Bookmarks;
            }
        }

        public void AddBookmark(Bookmark bookmark)
        {
            _Bookmarks.Add(bookmark);
        }

        public void RemoveBookmark(int bookmarkId)
        {
            var target = _Bookmarks.FirstOrDefault((t) => t.Id == bookmarkId);
            _Bookmarks.Remove(target);
        }



        #endregion

        #region 第一个标签

        /// <summary>
        /// 第一个标签
        /// </summary>
        [PropertyRepository(Lazy =true)]
        private static readonly DomainProperty FirstBookmarkProperty = DomainProperty.Register<Bookmark, Book>("FirstBookmark", Bookmark.Empty);

        public Bookmark FirstBookmark
        {
            get
            {
                return GetValue<Bookmark>(FirstBookmarkProperty);
            }
            set
            {
                SetValue(FirstBookmarkProperty, value);
            }
        }


        #endregion

        #region 最后一个标签

        /// <summary>
        /// 引用对象
        /// </summary>
        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty LastBookmarkProperty = DomainProperty.Register<Bookmark, Book>("LastBookmark", Bookmark.Empty);

        /// <summary>
        /// 书签
        /// </summary>
        public Bookmark LastBookmark
        {
            get
            {
                return GetValue<Bookmark>(LastBookmarkProperty);
            }
            set
            {
                SetValue(LastBookmarkProperty, value);
            }
        }

        #endregion

        #region 海报

        /// <summary>
        /// 值对象的集合
        /// </summary>
        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty PostersProperty = DomainProperty.RegisterCollection<BookPoster, Book>("Posters");

        private DomainCollection<BookPoster> _Posters
        {
            get
            {
                return GetValue<DomainCollection<BookPoster>>(PostersProperty);
            }
            set
            {
                SetValue(PostersProperty, value);
            }
        }

        public IEnumerable<BookPoster> Posters
        {
            get
            {
                return _Posters;
            }
        }


        public void AddPoster(BookPoster poster)
        {
            _Posters.Add(poster);
        }

        #endregion

        #region 错误页序号

        private static readonly DomainProperty ErrorPageIndexsProperty = DomainProperty.RegisterCollection<int, Book>("ErrorPageIndexs");

        private DomainCollection<int> _ErrorPageIndexs
        {
            get
            {
                return GetValue<DomainCollection<int>>(ErrorPageIndexsProperty);
            }
            set
            {
                SetValue(ErrorPageIndexsProperty, value);
            }
        }

        /// <summary>
        /// 书中错误页的序号
        /// </summary>
        [PropertyRepository()]
        public IEnumerable<int> ErrorPageIndexs
        {
            get
            {
                return _ErrorPageIndexs;
            }
            set
            {
                _ErrorPageIndexs = new DomainCollection<int>(ErrorPageIndexsProperty, value);
            }
        }



        #endregion

        #region 相关分类

        /// <summary>
        /// 相关分类
        /// </summary>
        [PropertyRepository(Lazy = true, Snapshot = true)]
        private static readonly DomainProperty RelatedCategoriesProperty = DomainProperty.RegisterCollection<BookCategory, Book>("RelatedCategories");

        private DomainCollection<BookCategory> _RelatedCategories
        {
            get
            {
                return GetValue<DomainCollection<BookCategory>>(RelatedCategoriesProperty);
            }
            set
            {
                SetValue(RelatedCategoriesProperty, value);
            }
        }

        public IEnumerable<BookCategory> RelatedCategories
        {
            get
            {
                return _RelatedCategories;
            }
        }


        public void AddRelatedCategory(BookCategory category)
        {
            this._RelatedCategories.Add(category);
        }


        public void AddSaleAddress(BookAddress address)
        {
            _SaleAddresses.Add(address);
        }



        #endregion

        #region 书的来源地址

        /// <summary>
        /// 书的来源地址
        /// </summary>
        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty SourceAddressProperty = DomainProperty.Register<BookAddress, Book>("SourceAddress", (owner) => BookAddress.Empty);

        public BookAddress SourceAddress
        {
            get
            {
                return GetValue<BookAddress>(SourceAddressProperty);
            }
            set
            {
                SetValue(SourceAddressProperty, value);
            }
        }


        #endregion

        #region 书的销售地址

        [PropertyRepository(Lazy = true)]
        private static readonly DomainProperty SaleAddressesProperty = DomainProperty.RegisterCollection<BookAddress, Book>("SaleAddresses");


        public IEnumerable<BookAddress> SaleAddresses
        {
            get
            {
                return _SaleAddresses;
            }
        }

        /// <summary>
        /// 销售地址
        /// </summary>
        private DomainCollection<BookAddress> _SaleAddresses
        {
            get
            {
                return GetValue<DomainCollection<BookAddress>>(SaleAddressesProperty);
            }
            set
            {
                SetValue(SaleAddressesProperty, value);
            }
        }

        #endregion

        #region 出版时间

        /// <summary>
        /// 出版日期
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty PublicationProperty = DomainProperty.Register<Emptyable<DateTime>, Book>("Publication");

        public Emptyable<DateTime> Publication
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(PublicationProperty);
            }
            set
            {
                SetValue(PublicationProperty, value);
            }
        }

        #endregion

        #region 销售时间

        /// <summary>
        /// 销售时间
        /// </summary>
        [PropertyRepository()]
        private static readonly DomainProperty SaleTimeProperty = DomainProperty.Register<Emptyable<DateTime>, Book>("SaleTime");

        public Emptyable<DateTime> SaleTime
        {
            get
            {
                return GetValue<Emptyable<DateTime>>(SaleTimeProperty);
            }
            set
            {
                SetValue(SaleTimeProperty, value);
            }
        }

        #endregion


        private static readonly DomainProperty ContentProperty = DomainProperty.Register<string, Book>("Content");

        [PropertyRepository()]
        [PropertyGet("GetContent")]
        public string Content
        {
            get
            {
                return GetValue<string>(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        protected virtual string GetContent()
        {
            return "Book的内容";
        }


        #region 空对象

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookEmpty : Book
        {
            public BookEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        private static readonly Book Empty = new BookEmpty();

        #endregion


        public Book(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public Book(Guid id,
                    Bookmark mainBookmark,
                    BookCover cover,
                    DomainCollection<int> errorPageIndexs)
            : base(id)
        {
            this.MainBookmark = mainBookmark;
            this.Cover = cover;
            _ErrorPageIndexs = errorPageIndexs;
            this.OnConstructed();
        }
    }
}