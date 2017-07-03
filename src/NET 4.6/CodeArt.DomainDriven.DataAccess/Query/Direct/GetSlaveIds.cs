using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 获得中间表里的从编号
    /// </summary>
    internal class GetSlaveIds : QueryBuilder
    {
        /// <summary>
        /// 表达式针对的目标表
        /// </summary>
        public DataTable Target
        {
            get;
            private set;
        }

        private string _sql;

        private GetSlaveIds(DataTable target)
        {
            this.Target = target;
            _sql = GetSql();
        }

        protected override string GetName()
        {
            return InternalQuery;
        }

        protected override string Process(DynamicData param)
        {
            return _sql;
        }

        private string GetSql()
        {
            switch (SqlContext.GetDbType())
            {
                case DatabaseType.SQLServer: return GetSqlBySQLServer();
            }
            return null;
        }

        public static GetSlaveIds Create(DataTable target)
        {
            return _getInstance(target);
        }

        private static Func<DataTable, GetSlaveIds> _getInstance = LazyIndexer.Init<DataTable, GetSlaveIds>((table)=>
        {
            return new GetSlaveIds(table);
        });


        private string GetSqlBySQLServer()
        {
            DataTable slave = this.Target;
            DataTable master = slave.Master;
            DataTable root = slave.Root;
            DataTable middle = slave.Middle;

            var rootId = root.TableIdName;
            var slaveId = slave.TableIdName;

            StringBuilder sql = new StringBuilder();
            //if (root.IsEqualsOrDerivedOrInherited(master))
            if(root.IsEqualsOrDerivedOrInherited(master))
            {
                sql.AppendFormat("select [{1}] from [{2}] where [{2}].[{0}] = @{0} order by [{3}] asc",
                    rootId, slaveId, middle.Name, GeneratedField.OrderIndexName);
            }
            else
            {
                var masterId = master.TableIdName;

                sql.AppendFormat("select [{1}] from {2} where [{2}].[{0}] = @{0} and [{2}].[{3}]=@{3} order by [{4}] asc",
                    rootId, slaveId, middle.Name, masterId, GeneratedField.OrderIndexName);
            }
            return sql.ToString();
        }

    }
}
