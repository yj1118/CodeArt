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

            if (table.IsEnabledMultiTenancy)
            {
                //如果是多租户，需要追加租户字段
                sql.AppendLine("[TenantId] [bigint] NOT NULL,");
            }

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

            if (table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("_{0}", GeneratedField.TenantIdName);
            }

            foreach (var field in primaryKeys)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.Append("] PRIMARY KEY CLUSTERED (");
            if (table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("[{0}],", GeneratedField.TenantIdName);
            }
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

            if (table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("_{0}", GeneratedField.TenantIdName);
            }

            foreach (var field in clusteredIndexs)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.AppendFormat("] ON [{0}](", table.Name);

            if (table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("[{0}],", GeneratedField.TenantIdName);
            }

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

            if(table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("_{0}", GeneratedField.TenantIdName);
            }

            foreach (var field in nonclusteredIndexs)
            {
                sql.AppendFormat("_{0}", field.Name);
            }
            sql.AppendFormat("] ON [{0}](", table.Name);

            if (table.IsEnabledMultiTenancy)
            {
                sql.AppendFormat("[{0}],", GeneratedField.TenantIdName);
            }

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
            bool allowNull = field.Tip.IsEmptyable || field.IsAdditional;


            if (field.DbType == DbType.String)
            {
                var maxLength = field.Tip.GetMaxLength();
                var isASCII = field.Tip.IsASCIIString();
                var max = isASCII ? 8000 : 4000;
                return string.Format("[{0}] [{1}]({2}) {3} NULL,", field.Name,
                                                                   isASCII ? "varchar" : "nvarchar",
                                                                   (maxLength == 0 || maxLength > max) ? "max" : maxLength.ToString(),
                                                                   allowNull ? string.Empty : "NOT");
            }
            else if (field.DbType == DbType.AnsiString)
            {
                var maxLength = field.Tip.GetMaxLength();
                var isASCII = true;
                var max = isASCII ? 8000 : 4000;
                return string.Format("[{0}] [{1}]({2}) {3} NULL,", field.Name,
                                                                   isASCII ? "varchar" : "nvarchar",
                                                                   (maxLength == 0 || maxLength > max) ? "max" : maxLength.ToString(),
                                                                   allowNull ? string.Empty : "NOT");
            }
            else if (field.DbType == DbType.Decimal)
            {
                return string.Format("[{0}] [{1}](18, 2) {2} NULL,", field.Name,
                                              Util.GetSqlDbTypeString(field.DbType),
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

        public static string GetClearTableSql(string tableName)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("if ISNULL(object_id(N'[{0}]'),'') > 0", tableName);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat("TRUNCATE TABLE [{0}]", tableName);
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
            if (table.IsEnabledMultiTenancy)
            {
                if (table.Type == DataTableType.AggregateRoot)
                {
                    return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", table.Name, EntityObject.IdPropertyName, GeneratedField.TenantIdName);
                }
                else
                {
                    return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", table.Name, GeneratedField.RootIdName, GeneratedField.TenantIdName);
                }
            }
            else
            {
                if (table.Type == DataTableType.AggregateRoot)
                {
                    return string.Format("delete [{0}] where [{1}]=@{1};", table.Name, EntityObject.IdPropertyName);
                }
                else
                {
                    return string.Format("delete [{0}] where [{1}]=@{1};", table.Name, GeneratedField.RootIdName);
                }
            }
        }

        #endregion


        private static string GetDeleteMemberSql(DataTable table)
        {
            if(table.IsEnabledMultiTenancy)
            {
                return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2} and [{3}]=@{3};",
                            table.Name, GeneratedField.RootIdName, EntityObject.IdPropertyName, GeneratedField.TenantIdName);
            }
            else
            {
                return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};",
                            table.Name, GeneratedField.RootIdName, EntityObject.IdPropertyName);
            }
        }

        /// <summary>
        /// 删除中间表数据（根据参数判断由什么条件删除）
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static string GetDeleteMiddleSql(DataTable middle)
        {
            if(middle.IsPrimitiveValue)
            {
                return GetDeleteMiddleByPrimitiveValuesSql(middle);
            }

            var rootId = GeneratedField.RootIdName;
            var slaveId = GeneratedField.SlaveIdName;

            if (middle.Root == middle.Master)
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("if @{0} is null", rootId);  //根据slave删除
                sql.AppendLine();
                sql.AppendLine("begin");
                if(middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, slaveId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1};", middle.Name, slaveId);
                }
                
                sql.AppendLine();
                sql.AppendLine("return;");
                sql.AppendLine("end");

                sql.AppendFormat("if @{0} is null", slaveId);
                sql.AppendLine();
                if(middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1}  and [{2}]=@{2};", middle.Name, rootId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1};", middle.Name, rootId);
                }
                
                sql.AppendLine();
                sql.AppendLine("else");
                if(middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2} and [{3}]=@{3};", middle.Name, rootId, slaveId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, rootId, slaveId);
                }
                
                return sql.ToString();
            }
            else
            {
                var masterId = GeneratedField.MasterIdName;

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("if @{0} is null", rootId);  //根据slave删除
                sql.AppendLine();
                sql.AppendLine("begin");

                if(middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, slaveId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1};", middle.Name, slaveId);
                }

                
                sql.AppendLine();
                sql.AppendLine("return;");
                sql.AppendLine("end");

                sql.AppendFormat("if @{0} is null", slaveId);
                sql.AppendLine();
                if (middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2} and [{3}]=@{3};", middle.Name, rootId, masterId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, rootId, masterId);
                }
               
                sql.AppendLine();
                sql.AppendLine("else");
                if (middle.IsEnabledMultiTenancy)
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2}  and [{3}]=@{3};", middle.Name, rootId, slaveId, GeneratedField.TenantIdName);
                }
                else
                {
                    sql.AppendFormat("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, rootId, slaveId);
                }
                return sql.ToString();

            }
        }

        private static string GetDeleteMiddleByPrimitiveValuesSql(DataTable middle)
        {
            var rootId = GeneratedField.RootIdName;

            if (middle.Root == middle.Master)
            {
                if (middle.IsEnabledMultiTenancy)
                {
                    return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, rootId, GeneratedField.TenantIdName);
                }
                else
                {
                    return string.Format("delete [{0}] where [{1}]=@{1};", middle.Name, rootId);
                }
            }
            else
            {
                var masterId = GeneratedField.MasterIdName;

                if (middle.IsEnabledMultiTenancy)
                {
                    return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2} and [{3}]=@{3};", middle.Name, rootId, masterId, GeneratedField.TenantIdName);
                }
                else
                {
                    return string.Format("delete [{0}] where [{1}]=@{1} and [{2}]=@{2};", middle.Name, rootId, masterId);
                }
            }
        }


        #endregion

        #region 锁代码

        public static string GetLockCode(QueryLevel level)
        {
            return level.GetMSSqlLockCode();
        }

        #endregion

        public static string GetIncrementIdentitySql(string tableName)
        {
            string increment = string.Format("{0}Increment", tableName);

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendFormat("if(object_id('[{0}]') is null)", increment);
            sql.AppendLine("begin");
            sql.AppendLine("	create table [" + increment + "]([value] [bigint] NOT NULL,CONSTRAINT [PK_" + increment + "] PRIMARY KEY CLUSTERED ([value] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY];");
            sql.AppendLine("end");
            sql.AppendFormat("if(not exists(select 1 from [{0}] with(xlock,holdlock)))", increment);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat(" insert into [{0}](value) values(1);", increment);
            sql.AppendLine();
            sql.AppendLine(" select cast(1 as bigint) as value;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat(" update [{0}] set [value]=[value]+1;", increment);
            sql.AppendLine();
            sql.AppendFormat("select value from [{0}] with(nolock);", increment);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            return sql.ToString();
        }

        public static string GetSerialNumberSql(DataTable table)
        {
            if (table.IsEnabledMultiTenancy) return GetSerialNumberTenantSql(table);

            string sn = string.Format("{0}SerialNumber", table.Name);

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendFormat("if(object_id('[{0}]') is null)", sn);
            sql.AppendLine("begin");
            sql.AppendLine("	create table [" + sn + "]([value] [bigint] NOT NULL,CONSTRAINT [PK_" + sn + "] PRIMARY KEY CLUSTERED ([value] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY];");
            sql.AppendLine("end");
            sql.AppendFormat("if(not exists(select 1 from [{0}] with(xlock,holdlock)))", sn);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat(" insert into [{0}](value) values(1);", sn);
            sql.AppendLine();
            sql.AppendLine(" select cast(1 as bigint) as value;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat(" update [{0}] set [value]=[value]+1;", sn);
            sql.AppendLine();
            sql.AppendFormat("select value from [{0}] with(nolock);", sn);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            return sql.ToString();
        }


        private static string GetSerialNumberTenantSql(DataTable table)
        {
            string sn = string.Format("{0}SerialNumber", table.Name);

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("begin transaction;");
            sql.AppendFormat("if(object_id('[{0}]') is null)", sn);
            sql.AppendLine("begin");
            sql.AppendLine("	create table [" + sn + "]([TenantId] [bigint] NOT NULL,[value] [bigint] NOT NULL,CONSTRAINT [PK_" + sn + "] PRIMARY KEY CLUSTERED ([tenantId] ASC)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY];");
            sql.AppendLine("end");
            sql.AppendFormat("if(not exists(select 1 from [{0}] with(xlock,holdlock) where [tenantId]=@tenantId))", sn);
            sql.AppendLine();
            sql.AppendLine("begin");
            sql.AppendFormat(" insert into [{0}](tenantId,value) values(@tenantId,1);", sn);
            sql.AppendLine();
            sql.AppendLine(" select cast(1 as bigint) as value;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat(" update [{0}] set [value]=[value]+1 where [tenantId]=@tenantId;", sn);
            sql.AppendLine();
            sql.AppendFormat("select value from [{0}] with(nolock) where  [tenantId]=@tenantId;", sn);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendLine("commit;");
            return sql.ToString();
        }


        public static string GetSingleLockSql(DataTable table)
        {
            if (table.IsEnabledMultiTenancy)
            {
                return string.Format("select [{0}] from [{1}]{2} where [{0}]=@{0} and [{3}]=@{3};"
                                , EntityObject.IdPropertyName
                                , table.Name
                                , GetLockCode(QueryLevel.Single),
                                GeneratedField.TenantIdName);
            }
            else
            {
                return string.Format("select [{0}] from [{1}]{2} where [{0}]=@{0};"
                                , EntityObject.IdPropertyName
                                , table.Name
                                , GetLockCode(QueryLevel.Single));
            }
        }

    }
}
