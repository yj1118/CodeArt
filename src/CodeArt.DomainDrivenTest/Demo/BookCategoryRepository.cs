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
    public interface IBookCategoryRepository : IRepository<BookCategory>
    {

    }

    [SafeAccess]
    public class SqlBookCategoryRepository : SqlRepository<BookCategory>, IBookCategoryRepository
    {
        public static readonly IBookCategoryRepository Instance = new SqlBookCategoryRepository();

        static SqlBookCategoryRepository()
        {
        }
    }

}
