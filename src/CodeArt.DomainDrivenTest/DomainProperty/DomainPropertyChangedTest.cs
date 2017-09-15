using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using CodeArt.DomainDriven;
using CodeArt.TestTools;

using CodeArt.AppSetting;

namespace CodeArt.DomainDrivenTest
{
    [TestClass]
    public class DomainPropertyChangedTest : UnitTest
    {
        protected override void EachTestInitialize()
        {
            Symbiosis.Open();
        }

        protected override void EachTestCleanup()
        {
            Symbiosis.Close();
        }

        [TestMethod]
        public void ValueObjectHashCode()
        {
            var cover1 = new BookCover("照片1", "1");
            var cover2 = new BookCover("照片1", "1");
            Assert.IsTrue(cover1.GetHashCode() == cover2.GetHashCode());
            Assert.IsTrue(cover1.Equals(cover2));
        }

        [TestMethod]
        public void Common()
        {
            var category = new BookCategory(1)
            {
                Name = "分类1",
                Photo = new BookCover("照片1", "1"),
                Bookmark = new Bookmark(1)
                {
                    Description = "书签1",
                    PageIndex = 1
                }
            };

            category.MarkClean();

            //普通值得改变
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.NameProperty));
            category.Name = "分类1"; //值相同，不会触发改变
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.NameProperty));
            category.Name = "分类2";
            Assert.IsTrue(category.IsPropertyChanged(BookCategory.NameProperty));

            //值对象的改变
            //值对象改变，需要判断值对象中的成员是否发生改变
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.PhotoProperty));
            category.Photo = new BookCover("照片1", "1"); //值相同，不会触发改变
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.PhotoProperty));
            category.Photo = new BookCover("照片2", "2");//值相同，会触发改变
            Assert.IsTrue(category.IsPropertyChanged(BookCategory.PhotoProperty));

            Assert.IsFalse(category.IsPropertyChanged(BookCategory.BookmarkProperty));
            category.Bookmark = new Bookmark(1)
            {
                Description = "书签1",
                PageIndex = 1
            };
            //对于实体对象，只要赋值就改变，不论是否为同一个对象
            Assert.IsTrue(category.IsPropertyChanged(BookCategory.BookmarkProperty));
            category.MarkClean();
            Assert.IsFalse(category.Bookmark.IsChanged);

            //测试内聚根的实体对象属性发生改变，内聚根的属性不会标记被改变
            //这是因为内聚根和实体的引用关系并没有改变，所以不会标记被改变
            category.Bookmark.Description = "更改书签";
            Assert.IsTrue(category.Bookmark.IsChanged);
            Assert.IsTrue(category.Bookmark.IsPropertyChanged(Bookmark.DescriptionProperty));
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.BookmarkProperty));

            //内聚根有多个实体对象
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.CommentsProperty));
            var bookmark3 = new Bookmark(3)
            {
                PageIndex = 5,
                Description = "标签3"
            };
            category.AddComment(bookmark3);
            Assert.IsTrue(category.IsPropertyChanged(BookCategory.CommentsProperty));
            Assert.IsTrue(bookmark3.IsChanged);
            category.MarkClean();
            Assert.IsFalse(category.IsPropertyChanged(BookCategory.CommentsProperty));
            Assert.IsFalse(bookmark3.IsChanged);
        }

        public class BookCategory : AggregateRoot<BookCategory, int>
        {
            #region 静态成员

            [PropertyRepository]
            public static readonly DomainProperty NameProperty = null;

            [PropertyRepository]
            public static readonly DomainProperty PhotoProperty = null;

            [PropertyRepository]
            public static readonly DomainProperty BookmarkProperty = null;

            [PropertyRepository]
            public static readonly DomainProperty CommentsProperty = null;

            /// <summary>
            /// 空对象
            /// </summary>
            public static readonly BookCategory Empty;

            static BookCategory()
            {
                NameProperty = DomainProperty.Register<string, BookCategory>("Name");
                PhotoProperty = DomainProperty.Register<BookCover, BookCategory>("Photo", (owner) => BookCover.Empty);
                BookmarkProperty = DomainProperty.Register<Bookmark, BookCategory>("Bookmark", (owner) => Bookmark.Empty);
                CommentsProperty = DomainProperty.RegisterCollection<Bookmark, BookCategory>("Comments");
                Empty = new BookCategoryEmpty();
            }

            /// <summary>
            /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
            /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
            /// </summary>
            private class BookCategoryEmpty : BookCategory
            {
                public BookCategoryEmpty()
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

            [StringLength(1, 20)]
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

            public BookCover Photo
            {
                get
                {
                    return GetValue<BookCover>(PhotoProperty);
                }
                set
                {
                    SetValue(PhotoProperty, value);
                }
            }

            public Bookmark Bookmark
            {
                get
                {
                    return GetValue<Bookmark>(BookmarkProperty);
                }
                set
                {
                    SetValue(BookmarkProperty, value);
                }
            }

            public IEnumerable<Bookmark> Comments
            {
                get
                {
                    return _Comments;
                }
            }

            private DomainCollection<Bookmark> _Comments
            {
                get
                {
                    return GetValue<DomainCollection<Bookmark>>(CommentsProperty);
                }
                set
                {
                    SetValue(BookmarkProperty, value);
                }
            }

            public void AddComment(Bookmark comment)
            {
                _Comments.Add(comment);
            }



            public BookCategory(int id)
                : base(id)
            {
                this.OnConstructed();
            }
        }


        /// <summary>
        /// 书的封面
        /// </summary>
        [ObjectValidator()]
        public class BookCover : ValueObject
        {
            #region 静态成员

            public static readonly DomainProperty TitleProperty = null;

            public static readonly DomainProperty NumberProperty = null;

            /// <summary>
            /// 空对象
            /// </summary>
            public static readonly BookCover Empty;

            static BookCover()
            {
                TitleProperty = DomainProperty.Register<string, BookCover>("Title");
                NumberProperty = DomainProperty.Register<string, BookCover>("Number");

                Empty = new BookCoverEmpty();
            }

            /// <summary>
            /// 为了避免静态构造empty引起的循环依赖导致的BUG，我们创建单独的empty类
            /// 这个类可以在对象静态构造时，以new的方式构造新的empty对象，避免循环依赖
            /// </summary>
            private class BookCoverEmpty : BookCover
            {
                public BookCoverEmpty()
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
            /// 书号
            /// </summary>
            [PropertyRepository()]
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


            public BookCover(string title, string number)
            {
                this.Title = title;
                this.Number = number;
                this.OnConstructed();
            }

            public override bool IsEmpty()
            {
                return false;
            }
        }

        /// <summary>
        /// 书签
        /// </summary>
        [ObjectValidator()]
        public class Bookmark : EntityObject<Bookmark, int>
        {
            #region 静态成员

            [PropertyRepository]
            public static readonly DomainProperty PageIndexProperty = null;

            [PropertyRepository]
            public static readonly DomainProperty DescriptionProperty = null;

            /// <summary>
            /// 空对象
            /// </summary>
            public static readonly Bookmark Empty;

            static Bookmark()
            {
                PageIndexProperty = DomainProperty.Register<int, Bookmark>("PageIndex", 0);
                DescriptionProperty = DomainProperty.Register<string, Bookmark>("Description");

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

            public Bookmark(int id)
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
}
