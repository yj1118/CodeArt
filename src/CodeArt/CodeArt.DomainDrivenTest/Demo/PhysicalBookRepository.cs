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
    public interface IPhysicalBookRepository : IRepository<PhysicalBook>
    {
        //Page<Book> Finds(string name, int pageIndex, int pageSize);
    }

    [SafeAccess]
    public class SqlPhysicalBookRepository : SqlRepository<PhysicalBook>, IPhysicalBookRepository
    {

        //通过重新植入仓储，并定义与基类属性上相同的加载名称，就可以覆盖基类的加载方法
        //public static object GetContent(dynamic data)
        //{
        //    return "物理书的内容";
        //}

        public static readonly SqlPhysicalBookRepository Instance = new SqlPhysicalBookRepository();

        static SqlPhysicalBookRepository()
        {

        }
    }

}
