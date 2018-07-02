using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 书的读者
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class BookReader : EntityObject<BookReader, int>
    {
        #region 静态成员


        [StringLength(Min = 1, Max = 100)]
        [PropertyRepository()]
        public static readonly DomainProperty NameProperty = null;
        public static readonly DomainProperty SexProperty = null;


        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly BookReader Empty;

        static BookReader()
        {
            NameProperty = DomainProperty.Register<string, BookReader>("Name");
            SexProperty = DomainProperty.Register<Sex, BookReader>("Sex", Sex.Male);

            Empty = new BookReaderEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookReaderEmpty : BookReader
        {
            public BookReaderEmpty()
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


        [PropertyRepository()]
        public Sex Sex
        {
            get
            {
                return GetValue<Sex>(SexProperty);
            }
            set
            {
                SetValue(SexProperty, value);
            }
        }

        [ConstructorRepository]
        public BookReader(int id)
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