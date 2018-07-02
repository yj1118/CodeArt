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
    public interface ICarBrandRepository : IRepository<CarBrand>
    {

    }

    [SafeAccess]
    public class SqlCarBrandRepository : SqlRepository<CarBrand>, ICarBrandRepository
    {
        public static readonly ICarBrandRepository Instance = new SqlCarBrandRepository();

        static SqlCarBrandRepository()
        {
        }
    }

}
