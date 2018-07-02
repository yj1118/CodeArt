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
    public interface IAnimalRepository : IRepository<Animal>
    {

    }

    [SafeAccess]
    public class SqlAnimalRepository : SqlRepository<Animal>, IAnimalRepository
    {
        public static readonly IAnimalRepository Instance = new SqlAnimalRepository();

        static SqlAnimalRepository()
        {
        }
    }

}
