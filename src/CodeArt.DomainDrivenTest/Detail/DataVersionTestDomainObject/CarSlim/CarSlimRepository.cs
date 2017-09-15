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
    public interface ICarSlimRepository : IRepository<CarSlim>
    {

    }

    [SafeAccess]
    public class SqlCarSlimRepository : SqlRepository<CarSlim>, ICarSlimRepository
    {
        public static readonly ICarSlimRepository Instance = new SqlCarSlimRepository();

        static SqlCarSlimRepository()
        {
        }
    }

}
