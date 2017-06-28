using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;

namespace CodeArt.DomainDrivenTest.Demo
{
    public interface IBookRepository : IRepository<Book>
    {
        Page<Book> Finds(string name, int pageIndex, int pageSize);
    }

    [SafeAccess]
    public class SqlBookRepository : SqlRepository<Book>, IBookRepository
    {
        public Page<Book> Finds(string name, int pageIndex, int pageSize)
        {
            return this.Query<Book>("name like %@name%", pageIndex, pageSize, (data) =>
              {
                  data.Add("name", name);
              });
        }


        public static readonly IBookRepository Instance = new SqlBookRepository();

        static SqlBookRepository()
        {
            //SqlHelper.Create<Book>();
        }

        #region 加载

        //public static object GetContent(dynamic data)
        //{
        //    return "Book的内容";
        //}

        public static object LoadPerson(dynamic data)
        {
            //var personId = data.PersionId;
            return Person.Empty;
        }

        public static void SavePersion(Book book, dynamic data)
        {
            
        }


        public static object GetCategory(dynamic data)
        {
            return BookCategory.Empty;
        }

        public static object GetBookmarkCategory(dynamic data)
        {
            return BookmarkCategory.Empty;
        }

        public static object GetBookAddressCategory(dynamic data)
        {
            return BookCategory.Empty;
        }


        #endregion

    }

}
