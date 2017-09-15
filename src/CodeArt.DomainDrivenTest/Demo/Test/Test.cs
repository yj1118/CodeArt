using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    public class Test : DomainStage
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

        public static void FillBook(Book book)
        {
            var categoryMain = new BookCategory(15)
            {
                Name = "主要分类"
            };

            var category1 = new BookCategory(1)
            {
                Name = "分类1"
            };

            var category2 = new BookCategory(2)
            {
                Name = "分类2"
            };

            Author author1 = new Author("作者1", Sex.Female, Person.Empty);
            Author author2 = new Author("作者2", Sex.Male, Person.Empty);
            Author author3 = new Author("作者3", Sex.Male, Person.Empty);
            Author author4 = new Author("作者4", Sex.Female, Person.Empty);

            var autohrs = new Author[] { author1, author2, author3, author4 };


            var photo1 = new BookCover("照片1", "p1", new Author("照片1的作者", Sex.Male, Person.Empty), autohrs);
            var photo2 = new BookCover("照片2", "p2", new Author("照片2的作者", Sex.Female, Person.Empty), autohrs);
            var photo3 = new BookCover("照片3", "p3", new Author("照片3的作者", Sex.Female, Person.Empty), autohrs);
            var photo4 = new BookCover("照片4", "p4", new Author("照片4的作者", Sex.Male, Person.Empty), autohrs);
            var wuhanPhoto = new BookCover("武汉的照片", "1", new Author("小王", Sex.Male, Person.Empty), autohrs);
            var beijingPhoto = new BookCover("北京的照片", "2", new Author("老王", Sex.Male, Person.Empty), autohrs);
            var shangHaiPhoto = new BookCover("上海的照片", "3", new Author("小赵", Sex.Female, Person.Empty), autohrs);

            var wuhan = new BookAddress("武汉", BookCategory.Empty, new BookCover[]{
                wuhanPhoto,photo1,photo2
            });

            var beiJing = new BookAddress("北京", BookCategory.Empty, new BookCover[]{
                beijingPhoto,photo2,photo3
            });

            var shangHai = new BookAddress("上海", BookCategory.Empty, new BookCover[]{
                shangHaiPhoto,photo1,photo3
            });

            var bookMark1 = new Bookmark(1)
            {
                PageIndex = 1,
                Cover = new BookCover("书签1的封面", "1", new Author("书签1的封面的作者", Sex.Female, Person.Empty), Array.Empty<Author>())
            };
            bookMark1.AddCover(photo1);
            bookMark1.AddCover(photo2);

            var bookMark18 = new Bookmark(2)
            {
                PageIndex = 18,
            };
            bookMark18.AddCover(photo1);
            bookMark18.AddCover(photo3);

            var bookMark20 = new Bookmark(3)
            {
                PageIndex = 20
            };
            bookMark20.AddCover(photo4);

            book.Name = "第一本书";
            book.Category = categoryMain;
            book.ErrorPageIndexs = new int[] { 1, 2, 3, 4, 5, 6 };
            book.SaleTime = DateTime.Now;

            book.SetSignature("第一本书的个性签名");
            book.SetCreator(new Author("第一本书的作者", Sex.Female, Person.Empty));

            book.SetSignature2("第二个个性签名");

            book.Cover = new BookCover("主要的封面", "1", new Author("作者", Sex.Female, Person.Empty), autohrs);

            //添加相关分类
            book.AddRelatedCategory(category1);
            book.AddRelatedCategory(category2);

            book.SourceAddress = wuhan;
            book.AddSaleAddress(beiJing);
            book.AddSaleAddress(shangHai);

            book.MainBookmark = bookMark1;
            book.FirstBookmark = bookMark1;
            book.LastBookmark = bookMark18;

            book.AddBookmark(bookMark1);
            book.AddBookmark(bookMark18);
            book.AddBookmark(bookMark20);

            book.AddPoster(new BookPoster("第1个海报", "第1个提供商"));
            book.AddPoster(new BookPoster("第2个海报", "第2个提供商"));
            book.AddPoster(new BookPoster("第3个海报", "第3个提供商"));

            var reader1 = new BookReader(1)
            {
                Name = "张三丰",
                Sex = Sex.Female
            };

            var reader2 = new BookReader(2)
            {
                Name = "周芷诺",
                Sex = Sex.Male
            };

            bookMark1.AddReader(reader1);
            bookMark1.AddReader(reader2);

            {
                var repository = Repository.Create<IBookCategoryRepository>();
                repository.Add(categoryMain);
                repository.Add(category1);
                repository.Add(category2);
            }

            {
                var repository = Repository.Create<IBookRepository>();
                repository.Add(book);
            }
        }


        private Book InsertBook()
        {
            Book book = new Book(Guid.NewGuid());
            FillBook(book);
            return book;
        }

        /// <summary>
        /// 验证保存的数据是否正确
        /// </summary>
        [TestMethod]
        public void ValidateInsert()
        {
            var book = this.Fixture.Get<Book>() as Book;
            AssertBook(book);
        }

        /// <summary>
        /// 验证引用内聚根
        /// </summary>
        [TestMethod]
        public void ValidateQuoteRoot()
        {
            var book = this.Fixture.Get<Book>() as Book;
            Assert.AreEqual("主要分类", book.Category.Name);


            {
                //假设其他线程已经更改了分类信息
                DataPortal.Direct<BookCategory>((conn) =>
                {
                    string sql = "update bookCategory set name=@name,dataVersion=dataVersion+1 where id=@id";
                    conn.Execute(sql, new { name = "修改后的名称", id = book.Category.Id });
                });
            }

            //当前线程的分类信息还是老的
            Assert.AreEqual("主要分类", book.Category.Name);
            //但是分类信息是快照
            Assert.AreEqual(true, book.Category.IsSnapshot);


            var task = Task.Factory.StartNew(() =>
            {
                //在别的线程里，是最新的
                var category = book.Category;
                Assert.AreEqual("修改后的名称", book.Category.Name);
                Assert.AreEqual(false, book.Category.IsSnapshot);
            });

            task.Wait();
        }


        [TestMethod]
        public void QueryLike()
        {
            {
                var books = DataPortal.Query<Book>("category.name like %@name%", (arg) =>
                {
                    arg.Add("name", "要分");
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());
            }

            {
                var books = DataPortal.Query<Book>("category.name like %@name", (arg) =>
                {
                    arg.Add("name", "要分");
                }, QueryLevel.None);

                Assert.AreEqual(0, books.Count());

                books = DataPortal.Query<Book>("category.name like %@name", (arg) =>
                {
                    arg.Add("name", "主要");
                }, QueryLevel.None);

                Assert.AreEqual(0, books.Count());

                books = DataPortal.Query<Book>("category.name like %@name", (arg) =>
                {
                    arg.Add("name", "分类");
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());


                books = DataPortal.Query<Book>("category.name like @name%", (arg) =>
                {
                    arg.Add("name", "主要");
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());


                books = DataPortal.Query<Book>("category.name like @name%", (arg) =>
                {
                    arg.Add("name", "分类");
                }, QueryLevel.None);

                Assert.AreEqual(0, books.Count());
            }
        }

        [TestMethod]
        public void QueryIn()
        {
            {
                var books = DataPortal.Query<Book>("mainBookmark.id in (1,2,3)", (arg) =>
                {
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());
            }

            {
                var books = DataPortal.Query<Book>("mainBookmark.id in @ids", (arg) =>
                {
                    arg.Add("ids", new int[] { 1, 2, 3 });
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());
            }

            {
                var books = DataPortal.Query<Book>("category.name like %@name% and mainBookmark.id in @ids", (arg) =>
                {
                    arg.Add("name", "要分");
                    arg.Add("ids", new int[] { 1, 2, 3 });
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());
            }

            {
                var books = DataPortal.Query<Book>("category.name like   %@name%  and mainBookmark.id in   @ids ", (arg) =>
                {
                    arg.Add("name", "要分");
                    arg.Add("ids", new int[] { 1, 2, 3 });
                }, QueryLevel.None);

                Assert.AreEqual(1, books.Count());
            }

            //books = DataPortal.Query<Book>("findByName")
            //                                .SqlServer("cover.author.name like @name")
            //                                .Oracle("cover.author.name like @name")
            //                                .Execute((arg) =>
            //                                {
            //                                    arg.Add("name", "%作%");
            //                                }, QueryLevel.None);

            //books = DataPortal.Query<Book>("findByName").SqlServer("cover.author.name like @name")
            //                    .MySQL("cover.author.name like @name")
            //                    .Oracle("cover.author.name like @name")
            //                    .Execute((arg) =>
            //                    {
            //                        arg.Add("name", "%作%");
            //                    }, QueryLevel.None);


            //books = DataPortal.Query<Book>("findByName")
            //                    .SqlServer("cover.author.name like @name",(arg)=>
            //                    {
            //                        arg.Add("name", "%作%");
            //                    })
            //                   .MySQL("cover.author.name like @name")
            //                   .Oracle("cover.author.name like @name")
            //                   .Execute((arg) =>
            //                   {
            //                       arg.Add("name", "%作%");
            //                   }, QueryLevel.None);


        }


        #region 断言


        public static void AssertBook(Book book)
        {
            //验证book表的数据
            DataPortal.Direct<Book>((conn) =>
            {
                var data = conn.QuerySingle("select * from book where id=@id", new { Id = book.Id });
                Assert.AreEqual(data.Name, book.Name);
                Assert.AreEqual(data.CoverId, book.Cover.Id);
                Assert.AreEqual(data.CategoryId, book.Category.Id);
                Assert.AreEqual(data.MainBookmarkId, book.MainBookmark.Id);
                Assert.AreEqual(data.FirstBookmarkId, book.FirstBookmark.Id);
                Assert.AreEqual(data.LastBookmarkId, book.LastBookmark.Id);
                Assert.AreEqual(data.Signature, book.GetSignature());
                Assert.AreEqual(data.Signature2, book.GetSignature2());

                AssertAuthor(book, book.GetCreator(), conn);

                string dataEPIs = data.ErrorPageIndexs;
                var epis = dataEPIs.Split(',').Select((t) => int.Parse(t.FromBase64()));
                AssertPro.AreEqual(epis, book.ErrorPageIndexs);

                Assert.AreEqual(data.SourceAddressId, book.SourceAddress.Id);

                Assert.IsNull(data.Publication);
                Assert.IsTrue(book.Publication.IsEmpty());
                AssertPro.AreEqualSecond(data.SaleTime, (DateTime)book.SaleTime);
            });

            //验证book的属性Cover对应的数据信息
            DataPortal.Direct<Book>((conn) =>
            {
                var cover = book.Cover;
                AssertBookCover(book, cover, conn);
            });

            DataPortal.Direct<Book>((conn) =>
            {
                AssertBookmark(book, book.MainBookmark, conn);

                var bookmarksData = conn.Query("select * from Book_Bookmarks where RootId=@RootId", new { RootId = book.Id });
                AssertBookmarks(book, bookmarksData, book.Bookmarks, conn);

                AssertBookmark(book, book.FirstBookmark, conn);
                AssertBookmark(book, book.LastBookmark, conn);
            });

            DataPortal.Direct<Book>((conn) =>
            {
                var datas = conn.Query("select * from Book_Posters where RootId=@RootId", new { RootId = book.Id });
                AssertBookPosters(book, datas, book.Posters, conn);
            });

            DataPortal.Direct<Book>((conn) =>
            {
                var datas = conn.Query("select * from Book_RelatedCategories where RootId=@RootId", new { RootId = book.Id });
                AssertBookCategories(datas, book.RelatedCategories, conn);
            });

            DataPortal.Direct<Book>((conn) =>
            {
                AssertBookAddress(book, book.SourceAddress, conn);
                var datas = conn.Query("select * from Book_SaleAddresses where RootId=@RootId", new { RootId = book.Id });
                AssertBookAddresses(book, datas, book.SaleAddresses, conn);
            });

        }


        #region 断言封面

        private static void AssertBookCover(Book book, BookCover cover, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookCover where RootId=@RootId and id=@id", new { RootId = book.Id, Id = cover.Id });
            if (cover.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Number, cover.Number);
            Assert.AreEqual(data.Title, cover.Title);

            //验证cover 的  Author属性的信息
            var author = cover.Author;
            AssertAuthor(book, author, conn);

            //查询中间表
            var authorsData = conn.Query("select * from BookCover_Authors where RootId=@RootId and MasterId=@MasterId", new { RootId = book.Id, MasterId = cover.Id });
            Assert.AreEqual(authorsData.Count(), cover.Authors.Count());
            foreach (dynamic temp in authorsData)
            {
                var authorId = temp.SlaveId;
                var item = cover.Authors.FirstOrDefault((t) =>
                {
                    return t.Id == authorId;
                });
                AssertAuthor(book, item, conn);
            }
        }

        private static void AssertBookCovers(Book book, IEnumerable<dynamic> datas, IEnumerable<BookCover> covers, IDbConnection conn)
        {
            Assert.AreEqual(datas.Count(), covers.Count());
            foreach (dynamic temp in datas)
            {
                var bookCoverId = temp.SlaveId;
                var item = covers.FirstOrDefault((t) =>
                {
                    return t.Id == bookCoverId;
                });
                AssertBookCover(book, item, conn);
            }
        }

        #endregion

        private static void AssertAuthor(Book book, Author obj, IDbConnection conn)
        {
            var data = conn.QuerySingle("select * from dbo.Author where RootId=@RootId and id=@id", new { RootId = book.Id, Id = obj.Id });
            Assert.AreEqual(data.Name, obj.Name);
            Assert.AreEqual(data.Sex, (byte)obj.Sex);
            Assert.AreEqual(data.PersonId, obj.Person.Id);
            AssertPersion(obj.Person, conn);
        }

        private static void AssertPersion(Person person, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from Person where id=@id", new { Id = person.Id });
            if (person.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Name, person.Name);
        }

        #region 断言书签

        private static void AssertBookmark(Book book, Bookmark obj, IDbConnection conn)
        {
            var data = conn.QuerySingle("select * from Bookmark where RootId=@RootId and id=@id", new { RootId = book.Id, Id = obj.Id });
            Assert.AreEqual(data.PageIndex, obj.PageIndex);
            Assert.AreEqual(data.Description, obj.Description);
            Assert.AreEqual(data.CategoryId, obj.Category.Id);
            AssertBookmarkCategory(obj.Category, conn);
            AssertBookCover(book, obj.Cover, conn);

            Assert.AreEqual(data.MainReaderId, obj.MainReader.Id);
            AssertBookReader(book, obj.MainReader, conn);

            //查询中间表
            var readersData = conn.Query("select * from Bookmark_Readers where RootId=@RootId and MasterId=@MasterId", new { RootId = book.Id, MasterId = obj.Id });
            Assert.AreEqual(readersData.Count(), obj.Readers.Count());
            foreach (dynamic temp in readersData)
            {
                var bookReaderId = temp.SlaveId;
                var item = obj.Readers.FirstOrDefault((t) =>
                {
                    return t.Id == bookReaderId;
                });
                AssertBookReader(book, item, conn);
            }

            //查询中间表
            var coversData = conn.Query("select * from Bookmark_Covers where RootId=@RootId and MasterId=@MasterId", new { RootId = book.Id, MasterId = obj.Id });
            AssertBookCovers(book, coversData, obj.Covers, conn);
        }

        private static void AssertBookmarks(Book book, IEnumerable<dynamic> datas, IEnumerable<Bookmark> bookmarks, IDbConnection conn)
        {
            Assert.AreEqual(datas.Count(), bookmarks.Count());
            foreach (dynamic temp in datas)
            {
                var bookmarkId = temp.SlaveId;
                var item = bookmarks.FirstOrDefault((t) =>
                {
                    return t.Id == bookmarkId;
                });
                AssertBookmark(book, item, conn);
            }
        }

        #endregion

        private static void AssertBookReader(Book book, BookReader obj, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookReader where  RootId=@RootId and id=@id", new { RootId = book.Id, Id = obj.Id });
            if (obj.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Name, obj.Name);
            Assert.AreEqual(data.Sex, (byte)obj.Sex);
        }

        private static void AssertBookmarkCategory(BookmarkCategory obj, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookmarkCategory where id=@id", new { Id = obj.Id });
            if (obj.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Name, obj.Name);
        }

        #region 书的海报

        private static void AssertBookPosters(Book book, IEnumerable<dynamic> datas, IEnumerable<BookPoster> posters, IDbConnection conn)
        {
            Assert.AreEqual(datas.Count(), posters.Count());
            foreach (dynamic temp in datas)
            {
                var bookPosterId = temp.SlaveId;
                var item = posters.FirstOrDefault((t) =>
                {
                    return t.Id == bookPosterId;
                });
                AssertBookPoster(book, item, conn);
            }
        }

        private static void AssertBookPoster(Book book, BookPoster poster, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookPoster where RootId=@RootId and id=@id", new { RootId = book.Id, Id = poster.Id });
            if (poster.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Title, poster.Title);
            Assert.AreEqual(data.ProviderCompany, poster.ProviderCompany);
        }

        #endregion

        #region 书的分类

        private static void AssertBookCategories(IEnumerable<dynamic> datas, IEnumerable<BookCategory> objs, IDbConnection conn)
        {
            Assert.AreEqual(datas.Count(), objs.Count());
            foreach (dynamic temp in datas)
            {
                var bookCategoryId = temp.SlaveId;
                var item = objs.FirstOrDefault((t) =>
                {
                    return t.Id == bookCategoryId;
                });
                AssertBookCategory(item, conn);
            }
        }

        private static void AssertBookCategory(BookCategory obj, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookCategory where id=@id", new { Id = obj.Id });
            if (obj.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Name, obj.Name);
        }


        #endregion

        #region 书的地址

        private static void AssertBookAddress(Book book, BookAddress obj, IDbConnection conn)
        {
            var data = conn.QueryFirstOrDefault("select * from BookAddress where  RootId=@RootId and id=@id", new { RootId = book.Id, Id = obj.Id });
            if (obj.IsEmpty())
            {
                Assert.IsNull(data);
                return;
            }
            Assert.AreEqual(data.Name, obj.Name);
            Assert.AreEqual(data.CategoryId, obj.Category.Id);
            AssertBookCategory(obj.Category, conn);

            var categoryDatas = conn.Query("select * from BookAddress_Categories where RootId=@RootId and MasterId=@MasterId", new { RootId = book.Id, MasterId = obj.Id });
            AssertBookCategories(categoryDatas, obj.Categories, conn);

            AssertBookCover(book, obj.Photo, conn);
            var photoDatas = conn.Query("select * from BookAddress_Photos where RootId=@RootId and MasterId=@MasterId", new { RootId = book.Id, MasterId = obj.Id });
            AssertBookCovers(book, photoDatas, obj.Photos, conn);
        }

        private static void AssertBookAddresses(Book book, IEnumerable<dynamic> datas, IEnumerable<BookAddress> addresses, IDbConnection conn)
        {
            Assert.AreEqual(datas.Count(), addresses.Count());
            foreach (dynamic temp in datas)
            {
                var bookAddressId = temp.SlaveId;
                var item = addresses.FirstOrDefault((t) =>
                {
                    return t.Id == bookAddressId;
                });
                AssertBookAddress(book, item, conn);
            }
        }


        #endregion


        #endregion


        [TestMethod]
        public void Update()
        {
            var repository = Repository.Create<IBookRepository>();

            //更改普通属性的值
            var book = this.Fixture.Get<Book>();
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
                var count = GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 3); //MainBookmark的引用属性，目前被引用3次

                book.MainBookmark = Bookmark.Empty;
                repository.Update(book);
                count = GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 2); //MainBookmark的引用属性，目前被引用2次

                book.FirstBookmark = Bookmark.Empty;
                repository.Update(book);
                count = GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 1); //MainBookmark的引用属性，目前被引用1次

                book.RemoveBookmark(id);
                repository.Update(book);
                count = GetAssociatedCount<Bookmark>(book, id);
                Assert.AreEqual(count, 0); //MainBookmark的引用属性，目前被引用0次
            }
        }

        /// <summary>
        /// 获得引用次数
        /// </summary>
        /// <returns></returns>
        public static int GetAssociatedCount<T>(Book book, object id)
        {
            var tableName = typeof(T).Name;
            string sql = string.Format("select AssociatedCount from {0} where RootId=@RootId and id=@id", tableName);
            int count = 0;
            DataPortal.Direct<Book>((conn) =>
            {
                var result = conn.ExecuteScalar(sql, new { RootId = book.Id, Id = id });
                if (result != null) count = (int)result;
            });
            return count;
        }


        [TestMethod]
        public void Read()
        {
            var book = this.Fixture.Get<Book>();
            var repository = Repository.Create<IBookRepository>();
            var target = repository.Find(book.Id, QueryLevel.None);
            AssertBook(target);
            Assert.AreEqual("Book的内容", target.Content);
        }

        [TestMethod]
        public void Delete()
        {
            var book = this.Fixture.Get<Book>();
            var model = DataModel.Create(typeof(Book));
            model.Delete(book);
        }
    }
}
