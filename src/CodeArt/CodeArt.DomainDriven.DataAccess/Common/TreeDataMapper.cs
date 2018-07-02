using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class TreeDataMapper : DataMapper
    {
        public TreeDataMapper()
        {
            InitSql();
        }

        private void InitSql()
        {
            _getUpdateLRForInsertSqlBySqlServer = LazyIndexer.Init<string, string>(GetUpdateLRForInsertSqlBySqlServer);
            _getUpdateLRForDeleteSqlBySqlServer = LazyIndexer.Init<string, string>(GetUpdateLRForDeleteSqlBySqlServer);
            _getFindParentsSqlBySqlServer = LazyIndexer.Init<string, string>(GetFindParentsSqlBySqlServer);
        }

        protected override IEnumerable<DbField> GetAttachFields(Type objectType, bool isSnapshot)
        {
            return new DbField[] { DbField.Create<int>("lft"), DbField.Create<int>("rgt"), DbField.Create("rootId", this.IdProperty.PropertyType) };
        }

        #region 新增节点

        public override void OnInserted(DomainObject obj, DataTable table)
        {
            DataContext.Current.OpenLock(QueryLevel.Single);
            var id = (obj as IEntityObject).GetIdentity();
            var parent = obj.GetValue<IEntityObject>(this.ParentProperty);

            if (SqlContext.GetDbType() == DatabaseType.SQLServer)
            {
                if (parent.IsEmpty())
                {
                    //自身为根
                    DataPortal.Direct(obj.ObjectType, (conn) =>
                    {
                        var sql = string.Format("update dbo.{0} set lft=@lft,rgt=@rgt,rootId=@id where id=@id;", table.Name);
                        conn.Execute(sql,new { lft=0,rgt=1,id });
                    });
                }
                else
                {
                    var parentId = parent.GetIdentity();
                    var sql = _getUpdateLRForInsertSqlBySqlServer(table.Name);
                    //显示开启锁
                    DataPortal.Direct(obj.ObjectType, (conn) =>
                    {
                        conn.Execute(sql, new { id, parentId });
                    });
                }
                return;
            }

            throw new MapperNotImplementedException(obj.ObjectType, "insert");
        }

        /// <summary>
        /// 获得左右值的sql
        /// </summary>
        private Func<string, string> _getUpdateLRForInsertSqlBySqlServer;

        private string GetUpdateLRForInsertSqlBySqlServer(string tableName)
        {
            var idType = SQLServer.Util.GetSqlDbTypeString(this.IdProperty.PropertyType);

            //这里获取lft,rgt的值，并且更新子节点的lft,rgt数据
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("declare @rootId {0}; --根编号", idType);
            sql.AppendLine();
            sql.AppendFormat("select @rootId=rootId from dbo.{0} where id=@parentId;", tableName); //得到根编号
            sql.AppendLine();
            sql.AppendFormat("select id from dbo.{0} with(xlock,holdlock) where rootId=@rootId; --锁整个树", tableName);
            sql.AppendLine();
            sql.AppendLine("declare @prgt int; --父节点的右值");
            sql.AppendFormat("select @prgt = rgt from dbo.{0} where id=@parentId;", tableName);
            sql.AppendLine();
            sql.AppendLine("if(@prgt is null)");
            sql.AppendLine("begin");
            sql.AppendLine("	set @prgt=0;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat("	update dbo.{0} set rgt=rgt+2 where rgt>=@prgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendFormat("	update dbo.{0} set lft=lft+2 where lft>=@prgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendFormat("update dbo.{0} set lft=@prgt,rgt=@prgt+1,rootId=@rootId where id=@id;",tableName);
            return sql.ToString();
        }


        #endregion

        #region 删除节点

        public override void OnPreDelete(DomainObject obj, DataTable table)
        {
            var id = (obj as IEntityObject).GetIdentity();
           
            if (SqlContext.GetDbType() == DatabaseType.SQLServer)
            {
                var sql = _getUpdateLRForDeleteSqlBySqlServer(table.Name);
                DataPortal.Direct(obj.ObjectType, (conn) =>
                {
                    var result = conn.Execute(sql, new { id });
                });
                return;
            }

            throw new MapperNotImplementedException(obj.ObjectType, "delete");
        }

        /// <summary>
        /// 更新删除一个节点后，所有相关的节点的LR值
        /// </summary>
        private Func<string, string> _getUpdateLRForDeleteSqlBySqlServer;

        private string GetUpdateLRForDeleteSqlBySqlServer(string tableName)
        {
            var idType = SQLServer.Util.GetSqlDbTypeString(this.IdProperty.PropertyType);

            //这里获取lft,rgt的值，并且更新子节点的lft,rgt数据
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("declare @rootId {0}; --根编号", idType);
            sql.AppendLine();
            sql.AppendLine("declare @lft int; --自身的左值");
            sql.AppendLine("declare @rgt int; --自身的右值");
            sql.AppendFormat("select @lft = lft,@rgt = rgt,@rootId=rootId from dbo.{0} where id=@id;", tableName); //先得到根编号
            sql.AppendLine();
            sql.AppendFormat("select id from dbo.{0} with(xlock,holdlock) where rootId=@rootId; --锁整个树", tableName);
            sql.AppendLine();

            sql.AppendLine("declare @disValue int; --需要更新的值");
            sql.AppendLine("set @disValue=@rgt-@lft+1;--牵连节点需要更新的值为 右值-左值+1");

            //更新其他节点的左值和右值
            sql.AppendFormat("update dbo.{0} set rgt=rgt-@disValue where rgt>=@rgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendFormat("update dbo.{0} set lft=lft-@disValue where lft>=@rgt and rootId=@rootId;", tableName);
            return sql.ToString();
        }

        #endregion

        /// <summary>
        /// 代表节点编号的领域属性
        /// </summary>
        protected abstract DomainProperty IdProperty
        {
            get;
        }

        /// <summary>
        /// 代表父节点的领域属性
        /// </summary>
        protected abstract DomainProperty ParentProperty
        {
            get;
        }

        #region 常用查询

        public override string Build(QueryBuilder builder, DynamicData param, DataTable table)
        {
            if (builder.Name == "findParents")
            {
                if (SqlContext.GetDbType() == DatabaseType.SQLServer)
                {
                    return _getFindParentsSqlBySqlServer(table.Name);
                }

                throw new MapperNotImplementedException(builder.ObjectType, "findParents");
            }
            return base.Build(builder, param, table);
        }

        private Func<string, string> _getFindParentsSqlBySqlServer;

        private string GetFindParentsSqlBySqlServer(string tableName)
        {
            var idType = SQLServer.Util.GetSqlDbTypeString(this.IdProperty.PropertyType);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("declare @rgt int,@lft int,@rootId {0};", idType);
            sb.AppendLine();
            sb.AppendFormat("select @lft = lft,@rgt=rgt,@rootId=rootId from dbo.{0} where id=@id;", tableName);
            sb.AppendLine();
            sb.AppendFormat("select {0} from dbo.{1} where lft<@lft and rgt>@rgt and rootId=@rootId order by lft;",KeyFields, tableName);
            return sb.ToString();
        }

        #endregion

    }
}
