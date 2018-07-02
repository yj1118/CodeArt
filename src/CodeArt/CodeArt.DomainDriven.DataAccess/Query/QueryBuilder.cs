using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class QueryBuilder : IQueryBuilder
    {
        private string _name;

        public string Name
        {
            get
            {
                if (_name == null) _name = GetName();
                return _name;
            }
        }

        protected abstract string GetName();


        public Type ObjectType
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为用户自定义的查询
        /// </summary>
        public abstract bool IsUser
        {
            get;
        }


        public QueryBuilder(Type objectType)
        {
            this.ObjectType = objectType;
        }

        /// <summary>
        /// 构建查询，获取执行的文本，在这个过程中有可能改变param的值
        /// </summary>
        /// <returns></returns>
        public string Build(DynamicData param, DataTable table)
        {
            //尝试用代理来构建
            var sql = AgentBuild(param, table);
            if (!string.IsNullOrEmpty(sql)) return sql;
            if (this.IsUser) throw new DataAccessException(string.Format(Strings.NoQuery, this.Name));
            //通过内部构建
            return InternalBuild(param);
        }

        /// <summary>
        /// 代理构建
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private string AgentBuild(DynamicData param, DataTable table)
        {
            var agent = SqlContext.GetAgent();
            return agent.Build(this, param, table);
        }

        /// <summary>
        /// 内部构建
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private string InternalBuild(DynamicData param)
        {
            string sql = Process(param);
            if (string.IsNullOrEmpty(sql))
                throw new NotSupportDatabaseException(this.GetType().Name, SqlContext.GetDbType());
            return sql;
        }


        protected abstract string Process(DynamicData param);

        /// <summary>
        /// 表示框架内部的查询
        /// </summary>
        public const string InternalQuery = "Internal Query";

    }
}
