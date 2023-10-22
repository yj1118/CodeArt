using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Concurrent;
using CodeArt.DomainDriven;
using CodeArt.DomainDriven.DataAccess;

namespace CodeArt.TestPlatform
{
    public interface IServiceInvokeRepository : IRepository<ServiceInvoke>
    {
       
    }

    [SafeAccess]
    public class SqlServiceInvokeRepository : SqlRepository<ServiceInvoke>, IServiceInvokeRepository
    {
        private SqlServiceInvokeRepository() { }

        public static readonly IServiceInvokeRepository Instance = new SqlServiceInvokeRepository();
    }

}
