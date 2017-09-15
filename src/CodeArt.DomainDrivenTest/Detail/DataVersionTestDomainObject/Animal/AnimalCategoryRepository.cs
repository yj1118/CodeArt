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
    public interface IAnimalCategoryRepository : IRepository<AnimalCategory>
    {
    }

    [SafeAccess]
    public class SqlAnimalCategoryRepository : SqlRepository<AnimalCategory>, IAnimalCategoryRepository
    {
        public static readonly SqlAnimalCategoryRepository Instance = new SqlAnimalCategoryRepository();

        static SqlAnimalCategoryRepository()
        {

        }
    }

}
