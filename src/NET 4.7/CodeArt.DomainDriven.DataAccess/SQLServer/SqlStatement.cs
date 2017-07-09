using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess.SQLServer
{
    internal static class SqlStatement
    {
        #region 创建表

        public static string GetCreateTableSql(DataTable table)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("if ISNULL(object_id(N'[{0}]'),'') = 0", table.Name);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat("	CREATE TABLE [{0}](", table.Name);
            sql.AppendLine();
            foreach (var field in table.Fields)
            {
                sql.AppendLine(GetFieldSql(field));
            }
            if (table.IsSnapshot)
            {
                //如果是快照，需要追加快照保存时间的字段
                sql.AppendLine("[SnapshotTime][datetime] NOT NULL,");
                sql.AppendLine("[SnapshotLifespan][int] NOT NULL,");
            }
            sql.AppendFormat("	{0})", GetPrimaryKeySql(table));
            sql.AppendLine();
            sql.AppendLine(GetClusteredIndexSql(table));
            sql.AppendLine(GetNonclusteredIndexSql(table));
            sql.Append("end");
            return sql.ToString();
        }

        private static string GetPrimaryKeySql(DataTable table)
        {
            var primaryKeys = table.PrimaryKeys;

            if (primaryKeys.Count() == 0) return string.Empty;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("CONSTRAINT [PK_{0}", table.Name);
            foreach (var field in primaryKeys)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.Append("] PRIMARY KEY CLUSTERED (");
            foreach (var field in primaryKeys)
            {
                sql.AppendFormat("[{0}],", field.Name);
            }
            sql.Length--;
            sql.Append(") ON [PRIMARY]");
            return sql.ToString();
        }

        private static string GetClusteredIndexSql(DataTable table)
        {
            var clusteredIndexs = table.ClusteredIndexs;

            if (clusteredIndexs.Count() == 0) return string.Empty;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("CREATE CLUSTERED INDEX [IX_{0}", table.Name);
            foreach (var field in clusteredIndexs)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.AppendFormat("] ON [{0}](", table.Name);
            foreach (var field in clusteredIndexs)
            {
                sql.AppendFormat("[{0}],", field.Name);
            }
            sql.Length--;
            sql.Append(")");
            return sql.ToString();
        }

        private static string GetNonclusteredIndexSql(DataTable table)
        {
            var nonclusteredIndexs = table.NonclusteredIndexs;

            if (nonclusteredIndexs.Count() == 0) return string.Empty;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("CREATE NONCLUSTERED INDEX [IX_{0}", table.Name);
            foreach (var field in nonclusteredIndexs)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.AppendFormat("] ON [{0}](", table.Name);
            foreach (var field in nonclusteredIndexs)
            {
                sql.AppendFormat("[{0}],", field.Name);
            }
            sql.Length--;
            sql.Append(")");
            return sql.ToString();
        }

        private static string GetFieldSql(IDataField field)
        {
            bool allowNull = field.Tip.IsEmptyable;

            if (field.DbType == DbType.String)
            {
                var maxLength = field.Tip.GetMaxLength();
                var isASCII = field.Tip.IsASCIIString();
                return string.Format("[{0}] [{1}]({2}) {3} NULL,", field.Name,
                                                                   isASCII ? "varchar" : "nvarchar",
                                                                   maxLength == 0 ? "max" : maxLength.ToString(),
                                                                   allowNull ? string.Empty : "NOT");
            }
            else
            {
                return string.Format("[{0}] [{1}] {2} NULL,", field.Name,
                                                              Util.GetSqlDbTypeString(field.DbType),
                                                               allowNull ? string.Empty : "NOT");
            }
        }

        #endregion

        #region 删除表

        public static string GetDropTableSql(string tableName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("if ISNULL(object_id(N'[{0}]'),'') > 0", tableName);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat("DROP TABLE [{0}]", tableName);
            sql.AppendLine();
            sql.Append("end");
            return sql.ToString();
        }

        #endregion

        #region 删除数据


        public static string GetDeleteSql(DataTable table)
        {
            switch (table.Type)
            {
                case DataTableType.AggregateRoot:
                    {
                        return GetDeleteRootSql(table);
                    }
                case DataTableType.ValueObject:
                case DataTableType.EntityObject:
                case DataTableType.EntityObjectPro:
                    {
                        return GetDeleteMemberSql(table);
                    }
                case DataTableType.Middle:
                    {
                        return GetDeleteMiddleSql(table);
                    }
            }
            return string.Empty;
        }

        #region 删除存放内聚根对象的表以及相关表的数据

        private static string GetDeleteRootSql(DataTable table)
        {
            List<string> sqls = new List<string>();
            using (var temp = TempIndex.Borrow())
            {
                var index = temp.Item;
                FillDeleteByRootIdSql(table, sqls, index);
            }

            StringBuilder code = new StringBuilder();
            foreach (var sql in sqls)
                code.AppendLine(sql);
            return code.ToString().Trim();
        }

        private static void FillDeleteByRootIdSql(DataTable table, List<string> sqls, TempIndex index)
        {
            foreach (var child in table.BuildtimeChilds)
            {
                if (!index.TryAdd(child)) continue; //已处理过

                if (child.Type == DataTableType.AggregateRoot) continue;//不删除引用的外部根表
                FillDeleteByRootIdSql(child, sqls, index);

                if (child.Middle != null)
                    FillDeleteByRootIdSql(child.Middle, sqls, index);
            }

            var selfSql = GetDeleteByRootIdSql(table);
            if (!sqls.Contains(selfSql))
                sqls.Add(selfSql);
        }

        private static string GetDeleteByRootIdSql(DataTable table)
        {
            if (table.Type == DataTableType.AggregateRoot)
            {
                return string.Format("delete [{0}] where [{1}]=@{1};", table.Name, EntityObject.IdPropertyName);
            }
            else
            {
                return string.Format("delete [{0}] where [{1}]=@{1};", table.Name, table.Root.TableIdName);
            }
        }

        #endregion


        private static string GetDeleteMemberSql(DataTable table)
        {
            return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};",
                                        table.Name, table.Root.TableIdName, EntityObject.IdPropertyName);
        }

        /// <summary>
        /// 删除中间表数据（根据参数判断由什么条件删除）
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string GetDeleteMiddleSql(DataTable middle)
        {
            var rootId = middle.RootField.Name;
            var slaveId = middle.SlaveField.Name;

            if (middle.Root == middle.Master)
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("if @{0} is null", slaveId);
                sql.AppendLine();
                sql.AppendFormat("delete [{0}] where [{1}]=@{1}",
                                             middle.Name, rootId);
                sql.AppendLine();
                sql.AppendLine("else");
                sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2}",
                                            middle.Name, rootId, slaveId);
                return sql.ToString();
            }
            else
            {
                var masterId = middle.Master.TableIdName;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("if @{0} is null", slaveId);
                sql.AppendLine();
                sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};",
                                            middle.Name, rootId, masterId);
                sql.AppendLine();
                sql.AppendLine("else");
                sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2}",
                                            middle.Name, rootId, slaveId);
                return sql.ToString();

            }
        }




        #endregion

        #region 锁代码

        public static string GetLockCode(QueryLevel level)
        {
            switch (level.Code)
            {
                case QueryLevel.ShareCode: return string.Empty;
                case QueryLevel.MirroringCode:
                case QueryLevel.SingleCode: return " with(xlock,rowlock)";
                case QueryLevel.HoldSingleCode: return " with(xlock,holdlock)";
                default:
                    return " with(nolock)";
            }
        }

        #endregion

        public static string GetIncrementIdentitySql(DataTable table)
        {
            const string increment = "Increment";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendFormat("if(object_id('[{0}]') is null)", increment);
            sql.AppendLine("begin");
            sql.AppendLine("	create table [" + increment + "]([type] [varchar](100) NOT NULL,[value] [bigint] NOT NULL,CONSTRAINT [PK_" + increment + "] PRIMARY KEY CLUSTERED ([type] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY];");
            sql.AppendLine("end");
            sql.AppendFormat("if(not exists(select 1 from [{0}] with(xlock,holdlock) where [type]='{1}'))", increment, table.Name);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat(" insert into [{0}](type,value) values('{1}',1);", increment, table.Name);
            sql.AppendLine();
            sql.AppendLine(" select cast(1 as bigint) as value;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat(" update [{0}] set [value]=[value]+1 where [type]='{1}';", increment, table.Name);
            sql.AppendLine();
            sql.AppendFormat("select value from [{0}] with(nolock) where [type]='{1}';", increment, table.Name);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            return sql.ToString();
        }

        public static string GetSingleLockSql(DataTable table)
        {
            return string.Format("select [{0}] from [{1}]{2} where [{0}]=@{0};"
                            , EntityObject.IdPropertyName
                            , table.Name
                            , GetLockCode(QueryLevel.Single));
        }

    }
}
