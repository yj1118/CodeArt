using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 书的发源地
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class BookAddress : ValueObject
    {
        #region 静态成员

        public static readonly DomainProperty NameProperty = null;
        public static readonly DomainProperty CategoryProperty = null;
        public static readonly DomainProperty CategoriesProperty = null;

        public static readonly DomainProperty PhotoProperty = null;

        public static readonly DomainProperty PhotosProperty = null;

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly BookAddress Empty;

        static BookAddress()
        {
            NameProperty = DomainProperty.Register<string, BookAddress>("Name");
            CategoryProperty = DomainProperty.Register<BookCategory, BookAddress>("Category", (owner) => BookCategory.Empty);
            CategoriesProperty = DomainProperty.RegisterCollection<BookCategory, BookAddress>("Categories");
            PhotoProperty = DomainProperty.Register<BookCover, BookAddress>("Photo", (owner) => BookCover.Empty);
            PhotosProperty = DomainProperty.RegisterCollection<BookCover, BookAddress>("Photos");
            Empty = new BookAddressEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookAddressEmpty : BookAddress
        {
            public BookAddressEmpty()
                : base(string.Empty, BookCategory.Empty, Array.Empty<BookCover>())
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
        [StringLength(1, 150)]
        public string Name
        {
            get
            {
                return GetValue<string>(NameProperty);
            }
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        [PropertyRepository()]
        [NotEmpty]
        public BookCategory Category
        {
            get
            {
                return GetValue<BookCategory>(CategoryProperty);
            }
            private set
            {
                SetValue(CategoryProperty, value);
            }
        }

        /// <summary>
        /// 相关分类
        /// </summary>
        [PropertyRepository()]
        public IEnumerable<BookCategory> Categories
        {
            get
            {
                return _Categories;
            }
        }

        public DomainCollection<BookCategory> _Categories
        {
            get
            {
                return GetValue<DomainCollection<BookCategory>>(CategoriesProperty);
            }
            set
            {
                SetValue(CategoriesProperty, value);
            }
        }

        [PropertyRepository()]
        [NotEmpty]
        public BookCover Photo
        {
            get
            {
                return GetValue<BookCover>(PhotoProperty);
            }
            private set
            {
                SetValue(PhotoProperty, value);
            }
        }

        [PropertyRepository()]
        public IEnumerable<BookCover> Photos
        {
            get
            {
                return _Photos;
            }
        }


        /// <summary>
        /// 相册
        /// </summary>
        private DomainCollection<BookCover> _Photos
        {
            get
            {
                return GetValue<DomainCollection<BookCover>>(PhotosProperty);
            }
            set
            {
                SetValue(PhotosProperty, value);
            }
        }

        public void AddPhoto(BookCover photo)
        {
            _Photos.Add(photo);
        }

        [ConstructorRepository()]
        public BookAddress(string name, 
                            BookCategory category,
                            [ParameterRepository(typeof(List<BookCover>))]
                            IEnumerable<BookCover> photos)
        {
            this.Name = name;
            this.Category = category;
            this._Photos = new DomainCollection<BookCover>(BookAddress.PhotosProperty, photos);
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
