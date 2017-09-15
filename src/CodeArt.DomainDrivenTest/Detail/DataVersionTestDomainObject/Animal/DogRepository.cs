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
    public interface IDogRepository : IRepository<Dog>
    {
    }

    [SafeAccess]
    public class SqlDogRepository : SqlRepository<Dog>, IDogRepository
    {
        public static readonly SqlDogRepository Instance = new SqlDogRepository();

        static SqlDogRepository()
        {

        }
    }

}
