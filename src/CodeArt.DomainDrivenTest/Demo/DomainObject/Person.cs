using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    [ObjectRepository(typeof(IBookRepository))]
    public class Person : AggregateRoot<Person, Guid>
    {
        #region 领域属性和领域行为

        public static readonly DomainProperty NameProperty = null;

        public static readonly Person Empty;

        static Person()
        {
            NameProperty = DomainProperty.Register<string, Person>("Name");
            Empty = new PersonEmpty();
        }


        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class PersonEmpty : Person
        {
            public PersonEmpty()
                : base(Guid.Empty)
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
        /// 名字
        /// </summary>
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

     
        [ConstructorRepository()]
        public Person(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }
    }
}
