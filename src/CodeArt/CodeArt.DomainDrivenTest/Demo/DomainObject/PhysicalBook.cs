using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDrivenTest.Demo
{
    [DerivedClass(typeof(PhysicalBook), "{048B3FD3-1000-4AD3-AAE0-BC1986FFAD4D}")]
    [ObjectRepository(typeof(IPhysicalBookRepository))]
    public class PhysicalBook : Book
    {
        private static readonly DomainProperty SubjectProperty = DomainProperty.Register<string,PhysicalBook>("Subject");



        /// <summary>
        /// 科目
        /// </summary>
        [PropertyRepository]
        [StringLength(1, 100)]
        public string Subject
        {
            get
            {
                return GetValue<string>(SubjectProperty);
            }
            set
            {
                SetValue(SubjectProperty, value);
            }
        }


        private static readonly DomainProperty TeacherCoverProperty = DomainProperty.Register<PersonCover, PhysicalBook>("TeacherCover");

        /// <summary>
        /// 老师的封面
        /// </summary>
        [PropertyRepository]
        public PersonCover TeacherCover
        {
            get
            {
                return GetValue<PersonCover>(TeacherCoverProperty);
            }
            set
            {
                SetValue(TeacherCoverProperty, value);
            }
        }

        private static readonly DomainProperty PhysicalCoverProperty = DomainProperty.Register<BookCover, PhysicalBook>("PhysicalCover");

        /// <summary>
        /// 物理书的专有封面
        /// </summary>
        [PropertyRepository]
        public BookCover PhysicalCover
        {
            get
            {
                return GetValue<BookCover>(PhysicalCoverProperty);
            }
            set
            {
                SetValue(PhysicalCoverProperty, value);
            }
        }



        private static readonly DomainProperty ProfessorCoverProperty = DomainProperty.Register<PersonCover, PhysicalBook>("ProfessorCover");

        /// <summary>
        /// 教授的封面
        /// </summary>
        [PropertyRepository]
        public PersonCover ProfessorCover
        {
            get
            {
                return GetValue<PersonCover>(ProfessorCoverProperty);
            }
            set
            {
                SetValue(ProfessorCoverProperty, value);
            }
        }

        protected override string GetContent()
        {
            return "物理书的内容";
        }

        private static readonly DomainProperty OwnerProperty = DomainProperty.RegisterDynamic<User, PhysicalBook>("Owner");

        /// <summary>
        /// 书的拥有者
        /// </summary>
        [PropertyRepository]
        public dynamic Owner
        {
            get
            {
                return GetValue<dynamic>(OwnerProperty);
            }
            set
            {
                SetValue(OwnerProperty, value);
            }
        }

        #region 空对象

        /// <summary>
        /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
        /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
        /// </summary>
        private class PhysicalBookEmpty : PhysicalBook
        {
            public PhysicalBookEmpty()
                : base(Guid.Empty)
            {
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return true;
            }
        }

        public static readonly PhysicalBook Empty = new PhysicalBookEmpty();

        #endregion

        public PhysicalBook(Guid id)
            : base(id)
        {
            this.OnConstructed();
        }

        [ConstructorRepository()]
        public PhysicalBook(Guid id,
                   Bookmark mainBookmark,
                   BookCover cover,
                   DomainCollection<int> errorPageIndexs)
            : base(id, mainBookmark, cover, errorPageIndexs)
        {
            this.OnConstructed();
        }

    }
}
