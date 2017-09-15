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
    /// 书的封面
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class BookCover : ValueObject
    {
     

        private static readonly DomainProperty TitleProperty = DomainProperty.Register<string, BookCover>("Title");

        /// <summary>
        /// 封面的标题
        /// </summary>
        [PropertyRepository(Snapshot =true)]
        [StringLength(1, 10)]
        public string Title
        {
            get
            {
                return GetValue<string>(TitleProperty);
            }
            private set
            {
                SetValue(TitleProperty, value);
            }
        }

        private static readonly DomainProperty NumberProperty = DomainProperty.Register<string, BookCover>("Number");

        /// <summary>
        /// 书号
        /// </summary>
        [PropertyRepository(Snapshot = true)]
        [StringLength(1, 10)]
        public string Number
        {
            get
            {
                return GetValue<string>(NumberProperty);
            }
            private set
            {
                SetValue(NumberProperty, value);
            }
        }



        private static readonly DomainProperty AuthorProperty = DomainProperty.Register<Author, BookCover>("Author");

        /// <summary>
        /// 作者
        /// </summary>
        [PropertyRepository()]
        [StringLength(1, 10)]
        public Author Author
        {
            get
            {
                return GetValue<Author>(AuthorProperty);
            }
            private set
            {
                SetValue(AuthorProperty, value);
            }
        }


        private static readonly DomainProperty AuthorsProperty = DomainProperty.RegisterCollection<Author, BookCover>("Authors");

        [PropertyRepository()]
        public IEnumerable<Author> Authors
        {
            get
            {
                return _Authors;
            }
        }

        private DomainCollection<Author> _Authors
        {
            get
            {
                return GetValue<DomainCollection<Author>>(AuthorsProperty);
            }
            set
            {
                SetValue(AuthorsProperty, value);
            }
        }


        [ConstructorRepository()]
        public BookCover(string title, string number, Author author, 
            [ParameterRepository(typeof(List<Author>))]
            IEnumerable<Author> authors)
        {
            this.Title = title;
            this.Number = number;
            this.Author = author;
            _Authors = new DomainCollection<Author>(BookCover.AuthorsProperty, authors);
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }



        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookCoverEmpty : BookCover
        {
            public BookCoverEmpty()
                : base(string.Empty, string.Empty, Author.Empty, Array.Empty<Author>())
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly BookCover Empty = new BookCoverEmpty();

    }
}
