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
    /// 作者
    /// </summary>
    [ObjectRepository(typeof(IBookRepository))]
    [ObjectValidator()]
    public class Author : ValueObject
    {
        #region 静态成员

        public static readonly DomainProperty NameProperty = null;
        public static readonly DomainProperty SexProperty = null;
        public static readonly DomainProperty PersonProperty = null;

        /// <summary>
        /// 空对象
        /// </summary>
        public static readonly Author Empty;

        static Author()
        {
            NameProperty = DomainProperty.Register<string, Author>("Name");
            SexProperty = DomainProperty.Register<Sex, Author>("Sex", Sex.Male);
            PersonProperty = DomainProperty.Register<Person, Author>("Person");

            Empty = new AuthorEmpty();
        }

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class AuthorEmpty : Author
        {
            public AuthorEmpty()
                : base(string.Empty, Sex.Male, Person.Empty)
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
            private set
            {
                SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// 性别
        /// </summary>
        [PropertyRepository()]
        [StringLength(1, 10)]
        public Sex Sex
        {
            get
            {
                return GetValue<Sex>(SexProperty);
            }
            private set
            {
                SetValue(SexProperty, value);
            }
        }


        [PropertyRepository()]
        public Person Person
        {
            get
            {
                return GetValue<Person>(PersonProperty);
            }
            private set
            {
                SetValue(PersonProperty, value);
            }
        }

        [ConstructorRepository()]
        public Author(string name, Sex sex, Person person)
        {
            this.Name = name;
            this.Sex = sex;
            this.Person = person;
            this.OnConstructed();
        }

        public override bool IsEmpty()
        {
            return false;
        }
    }
}
