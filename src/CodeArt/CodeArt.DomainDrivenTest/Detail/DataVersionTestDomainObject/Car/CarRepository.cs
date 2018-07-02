using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;

namespace CodeArt.DomainDrivenTest.Detail
{
    public interface ICarRepository : IRepository<Car>
    {

    }

    [SafeAccess]
    public class SqlCarRepository : SqlRepository<Car>, ICarRepository
    {
        public static readonly ICarRepository Instance = new SqlCarRepository();

        static SqlCarRepository()
        {
        }
    }

}
