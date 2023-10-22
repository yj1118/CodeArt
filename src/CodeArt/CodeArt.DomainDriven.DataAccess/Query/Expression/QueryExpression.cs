using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.DomainDriven;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 基于表达式的查询,可以指定对象属性等表达式
    /// select子语句系统内部使用，外部请不要调用
    /// </summary>
    public abstract class QueryExpression : QueryBuilder
    {
        /// <summary>
        /// 表达式针对的目标表
        /// </summary>
        public DataTable Target
        {
            get;
            private set;
        }


        public string Expression
        {
            get;
            private set;
        }

        public SqlDefinition Definition
        {
            get;
            private set;
        }


        /// <summary>
        /// 查询的锁定级别
        /// </summary>
        public QueryLevel Level
        {
            get;
            private set;
        }

        protected override string GetName()
        {
            return this.Definition.Key;
        }

        public override bool IsUser
        {
            get
            {
                return !string.IsNullOrEmpty(this.Definition.Key);
            }
        }


        public QueryExpression(DataTable target, string expression, QueryLevel level)
            : base(target.ObjectType)
        {
            this.Target = target;
            this.Expression = expression;
            this.Definition = SqlDefinition.Create(this.Expression, target.IsSessionEnabledMultiTenancy);
            this.Level = level;
        }

        protected override string Process(DynamicData param)
        {
            var commandText = GetCommandText(param);
            return this.Definition.Process(commandText, param);
        }

        /// <summary>
        /// 获取命令文本
        /// </summary>
        /// <param name="target"></param>
        /// <param name="param"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        protected abstract string GetCommandText(DynamicData param);

        protected string GetObjectSql()
        {
            var table = this.Target;

            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            sql.AppendLine(GetSelectFieldsSql(table, this.Definition));
            sql.AppendLine(" from ");
            sql.AppendLine(GetFromSql(table, this.Level, this.Definition));
            sql.Append(GetJoinSql(table, this.Definition));

            return GetFinallyObjectSql(sql.ToString(), table);
        }

        #region 得到select语句

        /// <summary>
        /// 获取表<paramref name="chainRoot"/>需要查询的select字段
        /// </summary>
        /// <param name="chainRoot"></param>
        /// <param name="exp"></param>
        /// <param name="chain">可以为输出的字段前置对象链</param>
        /// <returns></returns>
        private static string GetSelectFieldsSql(DataTable chainRoot, SqlDefinition exp)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(GetChainRootSelectFieldsSql(chainRoot, exp).Trim());

            using (var temp = TempIndex.Borrow())
            {
                var index = temp.Item;
                sql.Append(GetSlaveSelectFieldsSql(chainRoot, chainRoot, exp, index).Trim());
            }
            sql.Length--;//移除最后一个逗号
            return sql.ToString();
        }

        /// <summary>
        /// 填充查询链中根表的select的字段
        /// </summary>
        /// <param name="chainRoot"></param>
        /// <param name="exp"></param>
        /// <param name="sql"></param>
        private static string GetChainRootSelectFieldsSql(DataTable chainRoot, SqlDefinition exp)
        {
            StringBuilder sql = new StringBuilder();
            if (chainRoot.IsDerived)
            {
                FillChainRootSelectFieldsSql(chainRoot.InheritedRoot, TableType.InheritedRoot, exp, sql);

                foreach (var derived in chainRoot.Deriveds)
                {
                    FillChainRootSelectFieldsSql(derived, TableType.Derived, exp, sql);
                }
            }
            else
            {
                FillChainRootSelectFieldsSql(chainRoot, TableType.Common, exp, sql);
            }
            return sql.ToString();
        }

        private static void FillChainRootSelectFieldsSql(DataTable current, TableType tableType, SqlDefinition exp, StringBuilder sql)
        {
            sql.AppendLine();

            foreach (var field in current.Fields)
            {
                if (field.IsAdditional) continue; //不输出附加字段，有这类需求请自行编码sql语句，因为附加字段的定制化需求统一由数据映射器处理
                if (field.Tip.Lazy && !exp.SpecifiedField(field.Name)) continue;

                if (tableType == TableType.Derived)
                {
                    //派生表不输出主键信息
                    if (field.Name == EntityObject.IdPropertyName)
                        continue;

                    if (current.Type != DataTableType.AggregateRoot)
                    {
                        if (field.Name == GeneratedField.RootIdName)
                            continue;
                    }
                }

                if (!ContainsField(field.Name, exp)) continue;

                sql.AppendFormat("{0}.{1} as {1},", SqlStatement.Qualifier(current.Name),
                                                    SqlStatement.Qualifier(field.Name));
            }

            if(current.IsSessionEnabledMultiTenancy)
            {
                if (tableType != TableType.Derived)
                {
                    sql.AppendFormat("{0}.{1} as {1},", SqlStatement.Qualifier(current.Name),
                                                        SqlStatement.Qualifier(GeneratedField.TenantIdName));
                }
            }
        }

        /// <summary>
        /// 填充查询链中从表的select的字段
        /// </summary>
        /// <param name="chainRoot"></param>
        /// <param name="master"></param>
        /// <param name="exp"></param>
        /// <param name="sql"></param>
        private static string GetSlaveSelectFieldsSql(DataTable chainRoot, DataTable master, SqlDefinition exp, TempIndex index)
        {
            StringBuilder sql = new StringBuilder();
            if (master.IsDerived)
            {
                FillChildSelectFieldsSql(chainRoot, master.InheritedRoot, exp, sql, index);

                foreach (var derived in master.Deriveds)
                {
                    FillChildSelectFieldsSql(chainRoot, derived, exp, sql, index);
                }
            }
            else
            {
                FillChildSelectFieldsSql(chainRoot, master, exp, sql, index);
            }
            return sql.ToString();
        }

        private static void FillChildSelectFieldsSql(DataTable chainRoot, DataTable master, SqlDefinition exp, StringBuilder sql, TempIndex index)
        {
            foreach (var child in master.BuildtimeChilds)
            {
                if (!index.TryAdd(child)) continue; //防止由于循环引用导致的死循环

                if (child.IsDerived)
                {
                    FillFieldsSql(chainRoot, master, child.InheritedRoot, TableType.InheritedRoot, exp, sql, index);

                    foreach (var derived in child.Deriveds)
                    {
                        FillFieldsSql(chainRoot, master, derived, TableType.Derived, exp, sql, index);
                    }
                }
                else
                {
                    FillFieldsSql(chainRoot, master, child, TableType.Common, exp, sql, index);
                }
            }
        }

        private static void FillFieldsSql(DataTable chainRoot, DataTable master, DataTable current, TableType tableType, SqlDefinition exp, StringBuilder sql, TempIndex index)
        {
            if (!ContainsTable(chainRoot, exp, current)) return;

            var chain = current.GetChainCode(chainRoot);
            bool containsInner = exp.ContainsInner(chain);

            sql.AppendLine();

            foreach (var field in current.Fields)
            {
                if (field.IsAdditional) continue; //不输出附加字段，有这类需求请自行编码sql语句，因为附加字段的定制化需求统一由数据映射器处理
                if (field.Tip.Lazy && !exp.SpecifiedField(field.Name)) continue;

                if (tableType == TableType.Derived)
                {
                    if (field.Name == EntityObject.IdPropertyName || field.Name == GeneratedField.RootIdName)
                        continue;
                }

                var fieldName = string.Format("{0}_{1}", chain, field.Name);

                if (!containsInner &&
                        !ContainsField(fieldName, exp)) continue;

                sql.AppendFormat("{0}.{1} as {2},", SqlStatement.Qualifier(chain),
                                                    SqlStatement.Qualifier(field.Name),
                                                    SqlStatement.Qualifier(fieldName));
            }

            if (current.IsSessionEnabledMultiTenancy)
            {
                if (tableType != TableType.Derived)
                {
                    var fieldName = string.Format("{0}_{1}", chain, GeneratedField.TenantIdName);
                    sql.AppendFormat("{0}.{1} as {2},", SqlStatement.Qualifier(chain),
                                                        SqlStatement.Qualifier(GeneratedField.TenantIdName),
                                                        fieldName);
                }
            }

            FillChildSelectFieldsSql(chainRoot, current, exp, sql, index);
        }

        #endregion

        #region 获取from语句

        private static string GetFromSql(DataTable chainRoot, QueryLevel level, SqlDefinition exp)
        {
            if (chainRoot.IsDerived)
            {
                return GetFromSqlByDerived(chainRoot, level, exp);
                //return string.Format(" ({0}) as {1}", GetDerivedTableSql(chainRoot, level, string.Empty), chainRoot.Name);
            }
            else
            {
                return string.Format(" {0}{1}", SqlStatement.Qualifier(chainRoot.Name), GetLockCode(level));
            }
        }

        private static string GetFromSqlByDerived(DataTable table, QueryLevel level, SqlDefinition exp)
        {
            var inheritedRoot = table.InheritedRoot;

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(" {0}{1}", SqlStatement.Qualifier(inheritedRoot.Name), GetLockCode(level)); //inheritedRoot记录了条目信息，所以一定会参与查询
            foreach (var derived in table.Deriveds)
            {
                if (!exp.ContainsExceptId(derived)) continue;


                if (table.Type == DataTableType.AggregateRoot)
                {
                    sql.AppendFormat(" inner join {0}{2} on {1}.Id={0}.Id",
                        SqlStatement.Qualifier(derived.Name), SqlStatement.Qualifier(inheritedRoot.Name), GetLockCode(QueryLevel.None));
                }
                else
                {
                    sql.AppendFormat(" inner join {0}{3} on {1}.Id={0}.Id and {1}.{2}={0}.{2}",
                        SqlStatement.Qualifier(derived.Name)
                        , SqlStatement.Qualifier(inheritedRoot.Name)
                        , SqlStatement.Qualifier(GeneratedField.RootIdName)
                        , GetLockCode(QueryLevel.None));
                }
            }
            return sql.ToString();
        }

        #endregion

        #region 获取join语句

        private static string GetJoinSql(DataTable chainRoot, SqlDefinition exp)
        {
            StringBuilder sql = new StringBuilder();
            using (var temp = TempIndex.Borrow())
            {
                var index = temp.Item;
                FillJoinSql(chainRoot, chainRoot, exp, sql, index);
            }

            return sql.ToString();
        }

        private static void FillJoinSql(DataTable chainRoot, DataTable master, SqlDefinition exp, StringBuilder sql,TempIndex index)
        {
            if(master.IsDerived)
            {
                var inheritedRoot = master.InheritedRoot;
                FillChildJoinSql(chainRoot, inheritedRoot, exp, sql, index);
                foreach (var derived in master.Deriveds)
                {
                    FillChildJoinSql(chainRoot, derived, exp, sql, index);
                }
            }
            else
            {
                FillChildJoinSql(chainRoot, chainRoot, exp, sql, index);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainRoot">是查询的根表</param>
        /// <param name="master"></param>
        /// <param name="exp"></param>
        /// <param name="masterProxyName"></param>
        /// <param name="sql"></param>
        private static void FillChildJoinSql(DataTable chainRoot, DataTable master, SqlDefinition exp, StringBuilder sql, TempIndex index)
        {
            var masterChain = master.GetChainCode(chainRoot);

            foreach (var child in master.BuildtimeChilds)
            {
                if (!index.TryAdd(child)) continue; //防止由于循环引用导致的死循环

                if (child.IsDerived)
                {
                    FillJoinSqlByDerived(chainRoot, master, child, masterChain, exp, sql, index);
                }
                else
                {
                    FillJoinSqlByNoDerived(chainRoot, master, child, masterChain, exp, sql, index);
                }
            }
        }

        private static void FillJoinSqlByDerived(DataTable chainRoot, DataTable master, DataTable current, string masterChain, SqlDefinition exp, StringBuilder sql, TempIndex index)
        {
            if (!ContainsTable(chainRoot, exp, current)) return;

            var chain = current.GetChainCode(chainRoot);
            var childSql = GetDerivedTableSql(current, QueryLevel.None);
            string masterTableName = string.IsNullOrEmpty(masterChain) ? master.Name : masterChain;

            sql.AppendLine();
     
            var tip = current.MemberPropertyTip;
            sql.AppendFormat(" left join ({0}) as {1}{4} on {2}.{3}Id={1}.Id",
                                childSql, 
                                SqlStatement.Qualifier(chain), 
                                SqlStatement.Qualifier(masterTableName)
                                , tip.PropertyName, GetLockCode(QueryLevel.None));

            FillJoinSql(chainRoot, current, exp, sql, index);
        }


        private static void FillJoinSqlByNoDerived(DataTable chainRoot, DataTable master, DataTable current, string masterChain, SqlDefinition exp, StringBuilder sql, TempIndex index)
        {
            if (!ContainsTable(chainRoot, exp, current)) return;
            var chain = current.GetChainCode(chainRoot);
            string masterTableName = string.IsNullOrEmpty(masterChain) ? master.Name : masterChain;

            string currentTenantSql = current.IsSessionEnabledMultiTenancy ? string.Format("and {0}.{1}=@{2}", SqlStatement.Qualifier(chain),
                                                                SqlStatement.Qualifier(GeneratedField.TenantIdName),
                                                                GeneratedField.TenantIdName) : string.Empty;

            sql.AppendLine();
            if (current.IsMultiple)
            {
                var middle = current.Middle;
                var masterIdName = middle.Root == middle.Master ? GeneratedField.RootIdName : GeneratedField.MasterIdName;

                string middleTenantSql = middle.IsSessionEnabledMultiTenancy ? string.Format("and {0}.{1}=@{2}", SqlStatement.Qualifier(middle.Name),
                        SqlStatement.Qualifier(GeneratedField.TenantIdName),
                        GeneratedField.TenantIdName) : string.Empty;


                if (current.Type == DataTableType.AggregateRoot)
                {
                    sql.AppendFormat(" left join {0}{6} on {0}.{1}={2}.Id {7} left join {3} as {4}{6} on {0}.{5}={4}.Id {8}",
                    SqlStatement.Qualifier(middle.Name),
                    SqlStatement.Qualifier(masterIdName),
                    SqlStatement.Qualifier(masterTableName),
                    SqlStatement.Qualifier(current.Name),
                    SqlStatement.Qualifier(chain),
                    GeneratedField.SlaveIdName,
                    GetLockCode(QueryLevel.None),
                    middleTenantSql,
                    currentTenantSql);
                }
                else
                {
                    //中间的查询会多一个{4}.{6}={2}.Id的限定，
                    sql.AppendFormat(" left join {0}{7} on {0}.{1}={2}.Id {8} left join {3} as {4}{7} on {0}.{5}={4}.Id and {4}.{6}={2}.Id {9}",
                        SqlStatement.Qualifier(middle.Name),
                        SqlStatement.Qualifier(masterIdName),
                        SqlStatement.Qualifier(masterTableName),
                        SqlStatement.Qualifier(current.Name),
                        SqlStatement.Qualifier(chain),
                        GeneratedField.SlaveIdName,
                        GeneratedField.RootIdName,
                        GetLockCode(QueryLevel.None),
                        middleTenantSql,
                        currentTenantSql);
                }
            }
            else
            {
                if (current.Type == DataTableType.AggregateRoot)
                {
                    var tip = current.MemberPropertyTip;
                    sql.AppendFormat(" left join {0} as {1}{4} on {2}.{3}Id={1}.Id {5}",
                    SqlStatement.Qualifier(current.Name),
                    SqlStatement.Qualifier(chain),
                    SqlStatement.Qualifier(masterTableName),
                    tip.PropertyName,
                    GetLockCode(QueryLevel.None),
                    currentTenantSql);
                }
                else
                {
                    if(chainRoot.Type == DataTableType.AggregateRoot)
                    {
                        var chainRootMemberPropertyTip = current.ChainRoot.MemberPropertyTip;
                        //string rootTableName = chainRoot.Name;
                        string rootTableName = chainRootMemberPropertyTip == null ? chainRoot.Name : chainRootMemberPropertyTip.PropertyName;
                        var tip = current.MemberPropertyTip;
                        sql.AppendFormat(" left join {0} as {1}{4} on {2}.{3}Id={1}.Id and {1}.{5}={6}.Id {7}",
                        SqlStatement.Qualifier(current.Name),
                        SqlStatement.Qualifier(chain),
                        SqlStatement.Qualifier(masterTableName),
                        tip.PropertyName,
                        GetLockCode(QueryLevel.None),
                        GeneratedField.RootIdName,
                        SqlStatement.Qualifier(rootTableName),
                        currentTenantSql);
                    }
                    else
                    {
                        //查询不是从根表发出的，而是从引用表，那么直接用@RootId来限定
                        var tip = current.MemberPropertyTip;
                        sql.AppendFormat(" left join {0} as {1}{4} on {2}.{3}Id={1}.Id and {1}.{5}=@{5} {6}",
                        SqlStatement.Qualifier(current.Name),
                        SqlStatement.Qualifier(chain),
                        SqlStatement.Qualifier(masterTableName),
                        tip.PropertyName,
                        GetLockCode(QueryLevel.None),
                        GeneratedField.RootIdName,
                        currentTenantSql);
                    }
                    
                }
                    
            }
            
            FillChildJoinSql(chainRoot, current, exp, sql, index);
        }

        #endregion

        #region 其他辅助方法

        private enum TableType
        {
            InheritedRoot,
            Derived,
            Common
        }

        public static string GetLockCode(QueryLevel level)
        {
            var agent = SqlContext.GetAgent();
            if (agent.Database == DatabaseType.SQLServer)
            {
                return SQLServer.SqlStatement.GetLockCode(level);
            }
            throw new NotSupportDatabaseException("GetLockCode", agent.Database);
        }


        private static string GetDerivedTableSql(DataTable table, QueryLevel level)
        {
            var inheritedRoot = table.InheritedRoot;

            StringBuilder sql = new StringBuilder();
            sql.Append("select ");
            sql.Append(GetChainRootSelectFieldsSql(table, SqlDefinition.All));
            sql.Length--;
            sql.AppendLine();
            sql.AppendFormat(" from {0}{1}", SqlStatement.Qualifier(inheritedRoot.Name), GetLockCode(level));
            foreach (var derived in table.Deriveds)
            {
                string derivedTenantSql = derived.IsSessionEnabledMultiTenancy ? string.Format("and {0}.{1}=@{2}", SqlStatement.Qualifier(derived.Name),
                                                                SqlStatement.Qualifier(GeneratedField.TenantIdName),
                                                                GeneratedField.TenantIdName) : string.Empty;

                if (table.Type == DataTableType.AggregateRoot)
                {
                    sql.AppendFormat(" inner join {0}{2} on {1}.Id={0}.Id {3}",
                        SqlStatement.Qualifier(derived.Name), 
                        SqlStatement.Qualifier(inheritedRoot.Name), 
                        GetLockCode(QueryLevel.None), 
                        derivedTenantSql);
                }
                else
                {
                    sql.AppendFormat(" inner join {0}{3} on {1}.Id={0}.Id and {1}.{2}={0}.{2} {4}",
                        SqlStatement.Qualifier(derived.Name),
                        SqlStatement.Qualifier(inheritedRoot.Name),
                        SqlStatement.Qualifier(GeneratedField.RootIdName), 
                        GetLockCode(QueryLevel.None), 
                        derivedTenantSql);
                }
            }
            return sql.ToString();
        }


        /// <summary>
        /// 获取派生类table的完整代码，该代码可获取整个派生类的信息
        /// </summary>
        /// <param name="table"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        //private static string GetDerivedTableSql(DataTable table, QueryLevel level)
        //{
        //    var inheritedRoot = table.InheritedRoot;

        //    StringBuilder sql = new StringBuilder();
        //    sql.Append("select ");
        //    sql.AppendLine(GetSelectFieldsSql(table, SqlDefinition.All));
        //    sql.AppendFormat(" from {0}{1}", inheritedRoot.Name, GetLockCode(level));
        //    FillJoinSql(inheritedRoot, inheritedRoot, SqlDefinition.All, sql);
        //    foreach (var derived in table.Deriveds)
        //    {
        //        if (table.Type == DataTableType.AggregateRoot)
        //        {
        //            sql.AppendFormat(" inner join {0} on {1}.Id={0}.Id",
        //                derived.Name, inheritedRoot.Name);
        //        }
        //        else
        //        {
        //            sql.AppendFormat(" inner join {0} on {1}.Id={0}.Id and {1}.{2}={0}.{2}",
        //                derived.Name, inheritedRoot.Name, GeneratedField.RootIdName);
        //        }

        //        FillJoinSql(derived, derived, SqlDefinition.All, sql);
        //    }
        //    return sql.ToString();
        //}

        private static bool ContainsField(string fieldName, SqlDefinition exp)
        {
            if (exp.IsSpecifiedField)
            {
                return exp.ContainsField(fieldName);
            }
            return true;
        }

        private static bool ContainsTable(DataTable root, SqlDefinition exp, DataTable target)
        {
            var path = target.GetChainCode(root);
            bool containsInner = exp.ContainsInner(path);

            if (containsInner) return true;


            if (target.IsMultiple)
            {
                return exp.ContainsChain(path);
            }
            var tip = target.MemberPropertyTip;

            if (exp.IsSpecifiedField)
            {
                //指定了加载字段，那么就看表是否提供了相关的字段
                return exp.ContainsChain(path);
            }
            else
            {
                if (target.Type == DataTableType.AggregateRoot || tip.Lazy)
                {
                    if (!exp.ContainsChain(path))
                    {
                        return false; //默认情况下外部的内聚根、懒惰加载不连带查询
                    }
                }
                return true;
            }
        }


        //获取最终的输出代码
        private string GetFinallyObjectSql(string tableSql, DataTable table)
        {
            string sql = null;

            if(this.Definition.HasInner)
            {
                if (this.Definition.Condition.IsEmpty())
                {
                    sql = string.Format("select distinct * from ({0}) as {1}", tableSql, SqlStatement.Qualifier(table.Name));
                }
                else
                {
                    sql = string.Format("select distinct * from ({0}) as {1} where {2}", tableSql, SqlStatement.Qualifier(table.Name), this.Definition.Condition.Code);
                }
            }
            else
            {
                if (this.Definition.Condition.IsEmpty())
                {
                    sql = string.Format("select distinct {2} from ({0}) as {1}", tableSql, SqlStatement.Qualifier(table.Name), this.Definition.GetFieldsSql());
                }
                else
                {
                    sql = string.Format("select distinct {3} from ({0}) as {1} where {2}", tableSql, SqlStatement.Qualifier(table.Name), this.Definition.Condition.Code, this.Definition.GetFieldsSql());
                }
            }

            return string.Format("({0}) as {1}", sql, SqlStatement.Qualifier(table.Name));
        }

        #endregion
    }
}
