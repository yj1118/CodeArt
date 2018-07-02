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
    /// 书签
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class Bookmark : EntityObject<Bookmark, int>
    {
        #region 静态成员

        public static readonly DomainProperty PageIndexProperty = null;
        public static readonly DomainProperty DescriptionProperty = null;
        public static readonly DomainProperty CategoryProperty = null;
        public static readonly DomainProperty CoverProperty = null;

        public static readonly DomainProperty ReadersProperty = null;
        public static readonly DomainProperty MainReaderProperty = null;
        public static readonly DomainProperty CoversProperty = null;

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly Bookmark Empty;

        static Bookmark()
        {
            PageIndexProperty = DomainProperty.Register<int, Bookmark>("PageIndex", 0);
            DescriptionProperty = DomainProperty.Register<string, Bookmark>("Description");
            CategoryProperty = DomainProperty.Register<BookmarkCategory, Bookmark>("Category", (owner) => BookmarkCategory.Empty);
            CoverProperty = DomainProperty.Register<BookCover, Bookmark>("Cover", (owner) => BookCover.Empty);

            ReadersProperty = DomainProperty.RegisterCollection<BookReader, Bookmark>("Readers");
            MainReaderProperty = DomainProperty.Register<BookReader, Bookmark>("MainReader", (owner) => BookReader.Empty);
            CoversProperty = DomainProperty.RegisterCollection<BookCover, Bookmark>("Covers");

            Empty = new BookmarkEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookmarkEmpty : Bookmark
        {
            public BookmarkEmpty()
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
        public int PageIndex
        {
            get
            {
                return GetValue<int>(PageIndexProperty);
            }
            set
            {
                SetValue(PageIndexProperty, value);
            }
        }

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

        [PropertyRepository()]
        [NotEmpty]
        public BookmarkCategory Category
        {
            get
            {
                return GetValue<BookmarkCategory>(CategoryProperty);
            }
            set
            {
                SetValue(CategoryProperty, value);
            }
        }

        /// <summary>
        /// 值对象
        /// </summary>
        [PropertyRepository()]
        [NotEmpty]
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

        /// <summary>
        /// 读过该书签的读者
        /// </summary>
        [PropertyRepository()]
        [NotEmpty]
        public IEnumerable<BookReader> Readers
        {
            get
            {
                return _Readers;
            }
        }

        private DomainCollection<BookReader> _Readers
        {
            get
            {
                return GetValue<DomainCollection<BookReader>>(ReadersProperty);
            }
            set
            {
                SetValue(ReadersProperty, value);
            }
        }

        public void AddReader(BookReader reader)
        {
            _Readers.Add(reader);
        }


        /// <summary>
        /// 主要读者
        /// </summary>
        [PropertyRepository()]
        public BookReader MainReader
        {
            get
            {
                return GetValue<BookReader>(MainReaderProperty);
            }
            set
            {
                SetValue(MainReaderProperty, value);
            }
        }


        [PropertyRepository(Lazy = true)]
        [NotEmpty]
        public IEnumerable<BookCover> Covers
        {
            get
            {
                return _Covers;
            }
        }

        public void AddCover(BookCover cover)
        {
            _Covers.Add(cover);
        }


        private DomainCollection<BookCover> _Covers
        {
            get
            {
                return GetValue<DomainCollection<BookCover>>(CoversProperty);
            }
            set
            {
                SetValue(CoversProperty, value);
            }
        }

        public Bookmark(int id)
            : base(id)
        {
            this.OnConstructed();
        }


        [ConstructorRepository]
        public Bookmark(int id,
                [ParameterRepository(typeof(List<BookReader>))]
                IEnumerable<BookReader> readers)
            : base(id)
        {
            _Readers = new DomainCollection<BookReader>(Bookmark.ReadersProperty, readers);
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}