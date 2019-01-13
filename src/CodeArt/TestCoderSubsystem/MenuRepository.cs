using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;
using CodeArt.Concurrent;

namespace TestCoderSubsystem
{
    public interface IMenuRepository : IRepository<Menu>
    {

    }

    [SafeAccess]
    public class SqlMenuRepository : SqlRepository<Menu>, IMenuRepository
    {
        public static readonly SqlMenuRepository Instance = new SqlMenuRepository();

        static SqlMenuRepository()
        {
        }
    }

}
