using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Data;

using Dapper;

using CodeArt.TestTools;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.DomainDrivenTest.Demo;
using CodeArt.Runtime;
using CodeArt.Concurrent;
using CodeArt.Util;

namespace CodeArt.DomainDrivenTest.Demo
{
    [TestClass]
    public class ObjectInheritedTest : DomainStage
    {
        protected override void PreEnterScene()
        {
            DataPortal.RuntimeBuild();
        }

        protected override void LeftScene()
        {
            DataPortal.Dispose();
        }

        protected override void EnteredScene()
        {
            this.BeginTransaction();
            var book = InsertBook();
            this.Commit();
            this.Fixture.Add(book);
        }

        private static void AssertBook(PhysicalBook book)
        {
            Test.AssertBook(book);

            DataPortal.Direct<PhysicalBook>((conn) =>
            {
                var data = conn.QuerySingle("select * from PhysicalBook where id=@id", new { Id = book.Id });
                Assert.AreEqual(data.Subject, book.Subject);
            });

            Assert.AreEqual("物理书的内容", book.Content);
        }


        private PhysicalBook InsertBook()
        {
            PhysicalBook book = new PhysicalBook(Guid.NewGuid())
            {
                Subject = "光学",
                PhysicalCover = new BookCover("物理书的封面", "2", new Author("物理设计师", Sex.Female, Person.Empty), Array.Empty<Author>()),
                TeacherCover = new PersonCover("物理老师", "1", Author.Empty, Array.Empty<Author>(), "胡老师")
            };
            book.ProfessorCover = book.TeacherCover;

            var o = book.Owner;

            var id = 1;
            book.Owner = Repository.FindRemoteRoot<User>(id);
            AssertOwner(book.Owner);
            Test.FillBook(book);
            return book;
        }

        private void AssertOwner(dynamic owner)
        {
            Assert.AreEqual(1, owner.Id);
            Assert.AreEqual("袁俊", owner.Name);

            Assert.AreEqual("崽崽", owner.Son.Name);
            Assert.AreEqual("新星", owner.Wife.Name);
        }


        [TestMethod]
        public void ValidateInsertDerived()
        {
            var book = this.Fixture.Get<PhysicalBook>();
            AssertBook(book);

        }

        [TestMethod]
        public void ReadByBase()
        {
            var book = this.Fixture.Get<PhysicalBook>();
            {
                this.BeginTransaction(true);
                var repository = RepositoryFactory.Create<IBookRepository>();
                var target = repository.Find(book.Id, QueryLevel.None) as PhysicalBook;
                AssertBook(target);
                this.Commit();
            }
        }

        [TestMethod]
        public void ReadByDerived()
        {
            var book = this.Fixture.Get<PhysicalBook>();
            //直接对物理书查询的单元测试
            //测试底层会自动识别条件和内容取舍表，而不是所有情况下都连接所有的表
            this.BeginTransaction(true);
            var repository = RepositoryFactory.Create<IPhysicalBookRepository>();
            var target = repository.Find(book.Id, QueryLevel.None);
            AssertBook(target);
            this.Commit();
        }

        [TestMethod]
        public void QueryByDerived()
        {
            {
                var book = DataPortal.QuerySingle<PhysicalBook>("Subject like %@subject%", (arg) =>
                {
                    arg.Add("subject", "光");
                }, QueryLevel.None);

                AssertBook(book);
            }

            {
                var books = DataPortal.Query<PhysicalBook>("Subject like %@subject%[order by id]", 0, 20, (arg) =>
                  {
                      arg.Add("subject", "光");
                  });

                Assert.AreEqual(1, books.Objects.Count());
                AssertBook(books.Objects.First());
            }

            {
                var books = DataPortal.Query<PhysicalBook>("Subject like %@subject%[order by id]", 1, 20, (arg) =>
                {
                    arg.Add("subject", "光");
                });

                Assert.AreEqual(0, books.Objects.Count());
                Assert.AreEqual(1, books.DataCount);
            }

        }


        [TestMethod]
        public void UpdateByBase()
        {
            var repository = RepositoryFactory.Create<IBookRepository>();

            //更改普通属性的值
            var book = this.Fixture.Get<PhysicalBook>();
            book.Name = "新的名称";

            repository.Update(book);
            AssertBook(book);

            //更改引用成员的属性
            book.MainBookmark.PageIndex = 20;
            repository.Update(book);
            AssertBook(book);

            //判定引用次数
            {
                var id = book.MainBookmark.Id;
                var count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 3); //MainBookmark的引用属性，目前被引用3次

                book.MainBookmark = Bookmark.Empty;
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 2); //MainBookmark的引用属性，目前被引用2次

                book.FirstBookmark = Bookmark.Empty;
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 1); //MainBookmark的引用属性，目前被引用1次

                book.RemoveBookmark(id);
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 0); //MainBookmark的引用属性，目前被引用0次
            }
        }

        [TestMethod]
        public void UpdateByDerived()
        {
            this.BeginTransaction(true);

            var repository = Repository.Create<IPhysicalBookRepository>();

            //更改普通属性的值
            var book = this.Fixture.Get<PhysicalBook>();
            book.Name = "新的名称";
            book.Subject = "新的科目";

            repository.Update(book);

            this.Commit();

            var target = repository.Find(book.Id, QueryLevel.None);
            AssertBook(target);

            //更改引用成员的属性
            book.MainBookmark.PageIndex = 20;
            repository.Update(book);
            AssertBook(book);

            //判定引用次数
            {
                var id = book.MainBookmark.Id;
                var count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 3); //MainBookmark的引用属性，目前被引用3次

                book.MainBookmark = Bookmark.Empty;
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 2); //MainBookmark的引用属性，目前被引用2次

                book.FirstBookmark = Bookmark.Empty;
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 1); //MainBookmark的引用属性，目前被引用1次

                book.RemoveBookmark(id);
                repository.Update(book);
                count = Test.GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 0); //MainBookmark的引用属性，目前被引用0次
            }
        }
    }
}
