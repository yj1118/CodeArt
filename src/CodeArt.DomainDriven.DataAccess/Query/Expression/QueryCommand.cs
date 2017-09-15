using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public class QueryCommand : QueryExpression
    {
        private QueryCommand(DataTable target, string expression, QueryLevel level)
            : base(target, expression, level)
        {
        }

        protected override string GetCommandText(DynamicData param)
        {
            return string.Empty; //命令查询是不返回任何sql的，该查询会由mapper或者数据代理翻译
        }

        public static QueryCommand Create(DataTable target, string expression, QueryLevel level)
        {
            return _cache.GetInstance(target, expression, level);
        }

        private static ExpressionCache<QueryCommand> _cache = new ExpressionCache<QueryCommand>((target, expression, level) =>
        {
            return new QueryCommand(target, expression, level);
        });

    }
}
