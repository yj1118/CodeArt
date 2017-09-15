using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    /// <summary>
    /// 人物封面
    /// </summary>
    [DerivedClass(typeof(PersonCover), "{BD19B071-D838-488B-BDE4-D294FFFF6346}")]
    [ObjectRepository(typeof(IPhysicalBookRepository))]
    [ObjectValidator()]
    public class PersonCover : BookCover
    {
        private readonly static DomainProperty NameProperty = DomainProperty.Register<string, PersonCover>("Name");


        /// <summary>
        /// 人物名称
        /// </summary>
        [PropertyRepository]
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


        #region 空对象

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class PersonCoverEmpty : PersonCover
        {
            public PersonCoverEmpty()
                : base(string.Empty, string.Empty, Author.Empty, Array.Empty<Author>(),string.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public new static readonly PersonCover Empty = new PersonCoverEmpty();

        #endregion


        [ConstructorRepository()]
        public PersonCover(string title, string number, Author author,
                [ParameterRepository(typeof(List<Author>))]
                IEnumerable<Author> authors,string name)
            : base(title, number, author, authors)
        {
            this.Name = name;
            this.OnConstructed();
        }
    }
}
