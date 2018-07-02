using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace DomainDrivenTestApp.DomainModel
{
    public interface IUserRepository : IRepository<User>
    {

    }

    [SafeAccess]
    public class SqlUserRepository : SqlRepository<User>, IUserRepository
    {
        public static readonly SqlUserRepository Instance = new SqlUserRepository();

    }


}