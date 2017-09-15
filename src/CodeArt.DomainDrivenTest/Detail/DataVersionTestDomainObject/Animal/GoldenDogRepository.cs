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
    public interface IGoldenDogRepository : IRepository<GoldenDog>
    {
    }

    [SafeAccess]
    public class SqlGoldenDogRepository : SqlRepository<GoldenDog>, IGoldenDogRepository
    {
        public static readonly SqlGoldenDogRepository Instance = new SqlGoldenDogRepository();

        static SqlGoldenDogRepository()
        {

        }
    }

}
