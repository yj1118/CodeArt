using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 书的海报，每本书有多张海报
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class BookPoster : ValueObject
    {
        #region 静态成员

        public static readonly DomainProperty TitleProperty = null;
        public static readonly DomainProperty ProviderCompanyProperty = null;

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly BookPoster Empty;

        static BookPoster()
        {
            TitleProperty = DomainProperty.Register<string, BookPoster>("Title");
            ProviderCompanyProperty = DomainProperty.Register<string, BookPoster>("ProviderCompany");

            Empty = new BookPosterEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class BookPosterEmpty : BookPoster
        {
            public BookPosterEmpty()
                : base(string.Empty, string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        #endregion

        /// <summary>
        /// 海报的标语
        /// </summary>
        [PropertyRepository()]
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

        /// <summary>
        /// 提供商
        /// </summary>
        [PropertyRepository()]
        [StringLength(1, 10)]
        public string ProviderCompany
        {
            get
            {
                return GetValue<string>(ProviderCompanyProperty);
            }
            private set
            {
                SetValue(ProviderCompanyProperty, value);
            }
        }

        [ConstructorRepository]
        public BookPoster(string title,string providerCompany)
        {
            this.Title = title;
            this.ProviderCompany = providerCompany;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
