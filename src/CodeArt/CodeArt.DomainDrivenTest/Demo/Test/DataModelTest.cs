using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.TestTools;
using CodeArt.Concurrent;
using CodeArt.DomainDrivenTest.Demo;

namespace CodeArt.DomainDrivenTest.Demo
{
    [TestClass]
    public class DataModelTest : UnitTest
    {
        //public static void ClassInitialize(TestContext testContext)
        //{
        //    RepositoryFactory.Register<IBookRepository, Book>(SqlBookRepository.Instance);
        //}


        //[TestMethod]
        //public void GetDataModel()
        //{
        //    var model = DataModel.Create(typeof(Book));
        //    var bookTable = model.Root;

        //    Assert.AreEqual("Book", bookTable.Name);
        //    Assert.AreEqual(false, bookTable.IsMultiple);

        //    var fields = bookTable.Fields.ToArray();
        //    Assert.AreEqual(19, fields.Length);

        //    Assert.AreEqual("Id", fields[0].Name);
        //    Assert.AreEqual("Name", fields[1].Name);
        //    Assert.AreEqual("CategoryId", fields[2].Name);
        //    //内聚根中存在1个值对象(数据存根表)
        //    //由于Cover是Book内部的单个值对象，因此根表中保存
        //    Assert.AreEqual("Cover_Title", fields[3].Name);
        //    Assert.AreEqual("Cover_Number", fields[4].Name);
        //    Assert.AreEqual("Cover_Author_Name", fields[5].Name);
        //    Assert.AreEqual("Cover_Author_Sex", fields[6].Name);
        //    Assert.AreEqual("Cover_Author_PersonId", fields[7].Name);

        //    Assert.AreEqual("MainBookmarkId", fields[8].Name);
        //    Assert.AreEqual("FirstBookmarkId", fields[9].Name);
        //    Assert.AreEqual("LastBookmarkId", fields[10].Name);
        //    Assert.AreEqual("ErrorPageIndexs", fields[11].Name);
        //    Assert.AreEqual("SourceAddress_Name", fields[12].Name);
        //    Assert.AreEqual("SourceAddress_CategoryId", fields[13].Name);
        //    Assert.AreEqual("SourceAddress_Photo_Title", fields[14].Name);
        //    Assert.AreEqual("SourceAddress_Photo_Number", fields[15].Name);
        //    Assert.AreEqual("SourceAddress_Photo_Author_Name", fields[16].Name);
        //    Assert.AreEqual("SourceAddress_Photo_Author_Sex", fields[17].Name);
        //    Assert.AreEqual("SourceAddress_Photo_Author_PersonId", fields[18].Name);



        //    var bookTableChilds = bookTable.Childs.ToArray();
        //    Assert.AreEqual(13, bookTableChilds.Length);

        //    //内聚根中存在单个引用对象(数据存成员表)
        //    var mainBookmark = bookTableChilds[0]; //由于MainBookmark是Book内部的单个引用对象，因此在成员表中保存
        //    AssertBookmark(mainBookmark);
        //    Assert.AreEqual(mainBookmark.MemberField.GetPropertyName(), "MainBookmark");
        //    Assert.AreEqual(false, mainBookmark.IsMultiple);

        //    //内聚根Book中存在多个引用对象bookmarks(数据存成员表和中间表)
        //    //成员表
        //    var bookmarks = bookTableChilds[1];
        //    AssertBookmark(bookmarks);
        //    Assert.AreEqual(bookmarks.MemberField.GetPropertyName(), "Bookmarks");
        //    Assert.AreEqual(true, bookmarks.IsMultiple);
        //    //中间表
        //    var book_Bookmarks = bookTableChilds[2];
        //    AssertMiddleTable(book_Bookmarks, "Book", "Book", "Bookmark", "Bookmarks");


        //    var firstBookmark = bookTableChilds[3]; //由于firstBookmark是Book内部的单个引用对象，因此在成员表中保存
        //    AssertBookmark(firstBookmark);
        //    Assert.AreEqual(firstBookmark.MemberField.GetPropertyName(), "FirstBookmark");
        //    Assert.AreEqual(false, firstBookmark.IsMultiple);

        //    var lastBookmark = bookTableChilds[4]; //由于lastBookmark是Book内部的单个引用对象，因此在成员表中保存
        //    AssertBookmark(lastBookmark);
        //    Assert.AreEqual(lastBookmark.MemberField.GetPropertyName(), "LastBookmark");
        //    Assert.AreEqual(false, lastBookmark.IsMultiple);


        //    //内聚根中存在多个值对象(数据存成员表和中间表)
        //    var posters = bookTableChilds[5]; //由于posters是Book内部的多个值对象，因此在成员表中保存
        //    AssertBookPoster(posters);
        //    Assert.AreEqual(posters.MemberField.GetPropertyName(), "Posters");
        //    Assert.AreEqual(true, posters.IsMultiple);
        //    //中间表
        //    var book_Posters = bookTableChilds[6];
        //    AssertMiddleTable(book_Posters, "Book", "Book", "BookPoster", "Posters");



        //    //内聚根中存在多个根对象(数据存中间表)
        //    var book_relatedCategories = bookTableChilds[7];
        //    AssertMiddleTable(book_relatedCategories, "Book", "Book", "BookCategory", "RelatedCategories");

        //    //内聚根中的一个值对象存在多个根对象的引用(数据存中间表)
        //    var book_SourceAddress_Categories = bookTableChilds[8];
        //    AssertMiddleTable(book_SourceAddress_Categories, "Book", "Book", "BookCategory", "SourceAddress_Categories");


        //    //内聚根中的一个值对象(SourceAddress)存在多个值对象(Photos)的引用
        //    var bookCover = bookTableChilds[9];
        //    Assert.AreEqual("BookCover", bookCover.Name);
        //    AssertBookCover(bookCover);
        //    Assert.AreEqual(true, bookCover.IsMultiple);
        //    Assert.AreEqual(bookCover.MemberField.GetPropertyName(), "Photos");
        //    Assert.AreEqual(bookCover.MemberField.Parent.GetPropertyName(), "SourceAddress");
        //    //中间表
        //    var book_SourceAddress_Photots = bookTableChilds[10];
        //    AssertMiddleTable(book_SourceAddress_Photots, "Book", "Book", "BookCover", "SourceAddress_Photos");


        //    //内聚根中有多个值对象，每个值对象存在多个根对象的引用
        //    //值对象的表
        //    var bookAddress = bookTableChilds[11];
        //    AssertBookAddress(bookAddress);
        //    //中间表
        //    var book_SaleAddresses = bookTableChilds[12];
        //    AssertMiddleTable(book_SaleAddresses, "Book", "Book", "BookAddress", "SaleAddresses");

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="middle"></param>
        ///// <param name="rootName">根表名称</param>
        ///// <param name="masterName">主表名称</param>
        ///// <param name="slaveName">从表名称</param>
        //private void AssertMiddleTable(DataTable middle, string rootName, string masterName, string slaveName, string propertyName)
        //{
        //    Assert.AreEqual(string.Format("{0}_{1}", masterName, propertyName), middle.Name);
        //    Assert.IsNull(middle.MemberField);
        //    Assert.AreEqual(true, middle.IsMultiple);

        //    Assert.IsTrue(middle.Type == DataTableType.Middle);
        //    var fields = middle.Fields.ToList();

        //    if (rootName == masterName)
        //    {
        //        Assert.AreEqual(2, fields.Count);
        //        Assert.AreEqual(string.Format("{0}Id", rootName), fields[0].Name);
        //        Assert.AreEqual(string.Format("{0}Id", slaveName), fields[1].Name);
        //    }
        //    else
        //    {
        //        Assert.AreEqual(3, fields.Count);
        //        Assert.AreEqual(string.Format("{0}Id", rootName), fields[0].Name);
        //        Assert.AreEqual(string.Format("{0}Id", masterName), fields[1].Name);
        //        Assert.AreEqual(string.Format("{0}Id", slaveName), fields[2].Name);
        //    }
        //}


        //private void AssertBookmark(DataTable bookmark)
        //{
        //    Assert.AreEqual("Bookmark", bookmark.Name);
        //    Assert.AreEqual(11, bookmark.Fields.Count());
        //    var fields = bookmark.Fields.ToArray();
        //    Assert.AreEqual("BookId", fields[0].Name);
        //    Assert.AreEqual("Id", fields[1].Name);
        //    Assert.AreEqual("PageIndex", fields[2].Name);
        //    Assert.AreEqual("Description", fields[3].Name);
        //    //内聚根中存在单个引用对象中存在另外一个内聚根的引用(存字段)
        //    Assert.AreEqual("CategoryId", fields[4].Name);
        //    Assert.AreEqual("Cover_Title", fields[5].Name);
        //    Assert.AreEqual("Cover_Number", fields[6].Name);
        //    Assert.AreEqual("Cover_Author_Name", fields[7].Name);
        //    Assert.AreEqual("Cover_Author_Sex", fields[8].Name);
        //    Assert.AreEqual("Cover_Author_PersonId", fields[9].Name);

        //    Assert.AreEqual("MainReaderId", fields[10].Name);

            
        //    Assert.AreEqual(bookmark.Type, DataTableType.EntityObject);


        //    var childs = bookmark.Childs.ToArray();
        //    Assert.AreEqual(5, childs.Length);
        //    //引用对象中存在多个引用对象
        //    var readers = childs[0];
        //    AssertBookReader(readers);
        //    Assert.AreEqual(readers.MemberField.GetPropertyName(), "Readers");
        //    Assert.AreEqual(true, readers.IsMultiple);

        //    //中间表
        //    var bookmark_readers = childs[1];
        //    Assert.AreEqual("Bookmark_Readers", bookmark_readers.Name);
        //    var bookmark_readersFields = bookmark_readers.Fields.ToArray();
        //    Assert.AreEqual(3, bookmark_readersFields.Length);
        //    Assert.AreEqual("BookId", bookmark_readersFields[0].Name); //中间表，记录根编号
        //    Assert.AreEqual("BookmarkId", bookmark_readersFields[1].Name);
        //    Assert.AreEqual("BookReaderId", bookmark_readersFields[2].Name);
        //    Assert.AreEqual(true, bookmark_readers.IsMultiple);


        //    //引用对象中存在1个引用对象
        //    var mainReader = childs[2];
        //    AssertBookReader(mainReader);
        //    Assert.AreEqual(mainReader.MemberField.GetPropertyName(), "MainReader");
        //    Assert.AreEqual(false, mainReader.IsMultiple);

        //    //引用对象中存在多个值对象
        //    var covers = childs[3];
        //    AssertBookCover(covers);
        //    Assert.AreEqual(covers.MemberField.GetPropertyName(), "Covers");
        //    Assert.AreEqual(true, covers.IsMultiple);

        //    //中间表
        //    var bookmark_covers = childs[4];
        //    Assert.AreEqual("Bookmark_Covers", bookmark_covers.Name);
        //    Assert.AreEqual(true, bookmark_covers.IsMultiple);

        //    var bookmark_coversFields = bookmark_covers.Fields.ToArray();
        //    Assert.AreEqual(3, bookmark_coversFields.Length);
        //    Assert.AreEqual("BookId", bookmark_coversFields[0].Name);
        //    Assert.AreEqual("BookmarkId", bookmark_coversFields[1].Name);
        //    Assert.AreEqual("BookCoverId", bookmark_coversFields[2].Name);
        //}

        //private void AssertBookReader(DataTable reader)
        //{
        //    Assert.AreEqual("BookReader", reader.Name);
        //    Assert.AreEqual(4, reader.Fields.Count());

        //    var fields = reader.Fields.ToArray();
        //    Assert.AreEqual("BookId", fields[0].Name); //ORM追加的bookId
        //    Assert.AreEqual("Id", fields[1].Name);
        //    Assert.AreEqual("Name", fields[2].Name);
        //    Assert.AreEqual("Sex", fields[3].Name);

        //    Assert.AreEqual(reader.Type, DataTableType.EntityObject);
        //}


        //private void AssertBookPoster(DataTable poster)
        //{
        //    Assert.AreEqual("BookPoster", poster.Name);
        //    Assert.AreEqual(4, poster.Fields.Count());

        //    var fields = poster.Fields.ToArray();
        //    Assert.AreEqual("BookId", fields[0].Name);
        //    Assert.AreEqual("Id", fields[1].Name);   //这个id是自动追加的
        //    Assert.AreEqual("Title", fields[2].Name);
        //    Assert.AreEqual("ProviderCompany", fields[3].Name);

        //    Assert.AreEqual(typeof(Guid), fields[0].GetPropertyType());   //这个id是自动追加的
        //    Assert.AreEqual(poster.Type, DataTableType.ValueObject);
        //}


        //private void AssertBookCover(DataTable cover)
        //{
        //    Assert.AreEqual("BookCover", cover.Name);
        //    Assert.AreEqual(7, cover.Fields.Count());

        //    var fields = cover.Fields.ToArray();
        //    Assert.AreEqual("BookId", fields[0].Name);
        //    Assert.AreEqual("Id", fields[1].Name);   //这个id是自动追加的
        //    Assert.AreEqual("Title", fields[2].Name);
        //    Assert.AreEqual("Number", fields[3].Name);
        //    Assert.AreEqual("Author_Name", fields[4].Name);
        //    Assert.AreEqual("Author_Sex", fields[5].Name);
        //    Assert.AreEqual("Author_PersonId", fields[6].Name);


        //    Assert.AreEqual(typeof(Guid), fields[0].GetPropertyType());   //这个id是自动追加的
        //    Assert.AreEqual(cover.Type, DataTableType.ValueObject);
        //}

        //private void AssertBookAddress(DataTable bookAddress)
        //{
        //    Assert.AreEqual("BookAddress", bookAddress.Name);
        //    var fields = bookAddress.Fields.ToArray();
        //    Assert.AreEqual(9, fields.Length);
        //    Assert.AreEqual("BookId", fields[0].Name);
        //    Assert.AreEqual("Id", fields[1].Name);
        //    Assert.AreEqual("Name", fields[2].Name);
        //    Assert.AreEqual("CategoryId", fields[3].Name);
        //    Assert.AreEqual("Photo_Title", fields[4].Name);
        //    Assert.AreEqual("Photo_Number", fields[5].Name);
        //    Assert.AreEqual("Photo_Author_Name", fields[6].Name);
        //    Assert.AreEqual("Photo_Author_Sex", fields[7].Name);
        //    Assert.AreEqual("Photo_Author_PersonId", fields[8].Name);

     
        //    Assert.AreEqual(true, bookAddress.IsMultiple);

        //    var childs = bookAddress.Childs.ToArray();

        //    //值对象BookAddress引用多个根对象BookCategory
        //    var bookAddress_Categories = childs[0];
        //    Assert.AreEqual("BookAddress_Categories", bookAddress_Categories.Name);
        //    var bookAddress_CategoriesFields = bookAddress_Categories.Fields.ToArray();
        //    Assert.AreEqual(3, bookAddress_CategoriesFields.Length);
        //    Assert.AreEqual("BookId", bookAddress_CategoriesFields[0].Name);
        //    Assert.AreEqual("BookAddressId", bookAddress_CategoriesFields[1].Name);
        //    Assert.AreEqual("BookCategoryId", bookAddress_CategoriesFields[2].Name);
        //    Assert.AreEqual(true, bookAddress_Categories.IsMultiple);



        //    //值对象BookAddress引用多个值对象BookCover
        //    //首先由值对象的表需要存储
        //    var bookCover = childs[1];
        //    AssertBookCover(bookCover);
        //    Assert.AreEqual(bookCover.MemberField.GetPropertyName(), "Photos");

        //    //中间表
        //    var bookAddress_Photos = childs[2];
        //    Assert.AreEqual("BookAddress_Photos", bookAddress_Photos.Name);
        //    var bookAddress_PhotosFields = bookAddress_Photos.Fields.ToArray();
        //    Assert.AreEqual(3, bookAddress_PhotosFields.Length);
        //    Assert.AreEqual("BookId", bookAddress_PhotosFields[0].Name);
        //    Assert.AreEqual("BookAddressId", bookAddress_PhotosFields[1].Name);
        //    Assert.AreEqual("BookCoverId", bookAddress_PhotosFields[2].Name);
        //    Assert.AreEqual(true, bookAddress_Photos.IsMultiple);
        //}


        //[TestMethod]
        //public void GetTableModel()
        //{
        //    var model = DataModel.Create(typeof(Book));
        //    var tables = model.Tables.ToArray();

        //    Assert.AreEqual(16, tables.Length);

        //    var book = tables[0];
        //    Assert.AreEqual("Book", book.Name);

        //    var bookmark = tables[1];
        //    Assert.AreEqual("Bookmark", bookmark.Name);

        //    var bookReader = tables[2];
        //    Assert.AreEqual("BookReader", bookReader.Name);

        //    var bookmark_Readers = tables[3];
        //    Assert.AreEqual("Bookmark_Readers", bookmark_Readers.Name);

        //    var bookCover = tables[4];
        //    Assert.AreEqual("BookCover", bookCover.Name);

        //    var bookmark_Covers = tables[5];
        //    Assert.AreEqual("Bookmark_Covers", bookmark_Covers.Name);

        //    var book_Bookmarks = tables[6];
        //    Assert.AreEqual("Book_Bookmarks", book_Bookmarks.Name);

        //    var bookPoster = tables[7];
        //    Assert.AreEqual("BookPoster", bookPoster.Name);

        //    var book_Posters = tables[8];
        //    Assert.AreEqual("Book_Posters", book_Posters.Name);


        //    var book_RelatedCategories = tables[9];
        //    Assert.AreEqual("Book_RelatedCategories", book_RelatedCategories.Name);

        //    var book_sourceAddress_categories = tables[10];
        //    Assert.AreEqual("Book_SourceAddress_Categories", book_sourceAddress_categories.Name);

        //    var book_SourceAddress_Photos = tables[11];
        //    Assert.AreEqual("Book_SourceAddress_Photos", book_SourceAddress_Photos.Name);


        //    var bookAddress = tables[12];
        //    Assert.AreEqual("BookAddress", bookAddress.Name);

        //    var bookAddress_categories = tables[13];
        //    Assert.AreEqual("BookAddress_Categories", bookAddress_categories.Name);


        //    var bookAddress_Photos = tables[14];
        //    Assert.AreEqual("BookAddress_Photos", bookAddress_Photos.Name);

        //    var book_SaleAddresses = tables[15];
        //    Assert.AreEqual("Book_SaleAddresses", book_SaleAddresses.Name);
        //}
    }
}
