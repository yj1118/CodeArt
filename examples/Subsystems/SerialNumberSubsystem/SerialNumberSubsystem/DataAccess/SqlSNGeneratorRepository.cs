using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Concurrent;
using CodeArt.DomainDriven.DataAccess;

namespace SerialNumberSubsystem.DataAccess
{
    /// <summary>
    /// 流水号生成器的仓储
    /// </summary>
    [SafeAccess]
    public sealed class SqlSNGeneratorRepository : SqlRepository<SNGenerator>,ISNGeneratorRepository
    {
        public SNGenerator FindByMarkedCode(string markedCode, QueryLevel level)
        {
            return DataContext.Current.QuerySingle<SNGenerator>("markedCode=@markedCode", (arg) =>
            {
                arg.Add("markedCode", markedCode);
            }, level);
        }

        public SqlSNGeneratorRepository() { }

        public readonly static SqlSNGeneratorRepository Instance = new SqlSNGeneratorRepository();
    }
}
