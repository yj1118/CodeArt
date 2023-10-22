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
            _getUpdateLRForMoveSqlBySqlServer = LazyIndexer.Init<string, string>(GetUpdateLRForMoveSqlBySqlServer);
            _getFindParentsSqlBySqlServer = LazyIndexer.Init<string, string>(GetFindParentsSqlBySqlServer);
        }

        protected override IEnumerable<DbField> GetAttachFields(Type objectType, bool isSnapshot)
        {
            return new DbField[] { DbField.Create<int>("lft"), DbField.Create<int>("rgt"), DbField.Create("rootId", this.IdProperty.PropertyType), DbField.Create("moving", typeof(bool)) };
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
                        var sql = string.Format("update dbo.[{0}] set lft=@lft,rgt=@rgt,rootId=@id,moving=0 where id=@id;", table.Name);
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
            sql.AppendFormat("select @rootId=rootId from dbo.[{0}] where id=@parentId;", tableName); //得到根编号
            sql.AppendLine();
            sql.AppendFormat("select id from dbo.[{0}] with(xlock,holdlock) where rootId=@rootId; --锁整个树", tableName);
            sql.AppendLine();
            sql.AppendLine("declare @prgt int; --父节点的右值");
            sql.AppendFormat("select @prgt = rgt from dbo.[{0}] where id=@parentId;", tableName);
            sql.AppendLine();
            sql.AppendLine("if(@prgt is null)");
            sql.AppendLine("begin");
            sql.AppendLine("	set @prgt=0;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat("	update dbo.[{0}] set rgt=rgt+2 where rgt>=@prgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendFormat("	update dbo.[{0}] set lft=lft+2 where lft>=@prgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendLine("end");
            sql.AppendFormat("update dbo.[{0}] set lft=@prgt,rgt=@prgt+1,rootId=@rootId,moving=0 where id=@id;", tableName);
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
            sql.AppendFormat("select @lft = lft,@rgt = rgt,@rootId=rootId from dbo.[{0}] where id=@id;", tableName); //先得到根编号
            sql.AppendLine();
            sql.AppendFormat("select id from dbo.[{0}] with(xlock,holdlock) where rootId=@rootId; --锁整个树", tableName);
            sql.AppendLine();

            sql.AppendLine("declare @disValue int; --需要更新的值");
            sql.AppendLine("set @disValue=@rgt-@lft+1;--牵连节点需要更新的值为 右值-左值+1");

            //更新其他节点的左值和右值
            sql.AppendFormat("update dbo.[{0}] set rgt=rgt-@disValue where rgt>=@rgt and rootId=@rootId;", tableName);
            sql.AppendLine();
            sql.AppendFormat("update dbo.[{0}] set lft=lft-@disValue where lft>=@rgt and rootId=@rootId;", tableName);
            return sql.ToString();
        }

        #endregion

        #region 移动节点

        /// <summary>
        /// 将<paramref name="parentId"/>里的目录<paramref name="currentId"/>从移到目录<paramref name="targetId"/>里
        /// </summary>
        /// <param name="current"></param>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        public void Move(DomainObject current, DomainObject parent, DomainObject target)
        {
            Move(current, parent, target, null, null, null);
        }

        /// <summary>
        /// 将<paramref name="parentId"/>里的目录<paramref name="currentId"/>从移到目录<paramref name="targetId"/>里
        /// </summary>
        /// <param name="current"></param>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        public void Move(DomainObject current, DomainObject parent, DomainObject target, object arg0)
        {
            Move(current, parent, target, arg0, null, null);
        }


        /// <summary>
        /// 将<paramref name="parentId"/>里的目录<paramref name="currentId"/>从移到目录<paramref name="targetId"/>里
        /// </summary>
        /// <param name="current"></param>
        /// <param name="parent"></param>
        /// <param name="target"></param>
        /// <param name="arg0">arg0、arg1、arg2是给出的3个预留的业务参数，业务可以传递这3个值，在FillUpdateLRForMoveFirstSetpBeforeSql等方法里写自定代码</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Move(DomainObject current, DomainObject parent, DomainObject target, object arg0, object arg1, object arg2)
        {
            var currentId = (current as IEntityObject).GetIdentity();
            var parentId = (parent as IEntityObject).GetIdentity();
            var targetId = (target as IEntityObject).GetIdentity();

            if (SqlContext.GetDbType() == DatabaseType.SQLServer)
            {
                var sql = _getUpdateLRForMoveSqlBySqlServer(current.ObjectType.Name);
                DataPortal.Direct(current.ObjectType, (conn) =>
                {
                    var result = conn.Execute(sql, new { currentId, parentId, targetId, arg0, arg1, arg2 });
                });
                return;
            }

            throw new MapperNotImplementedException(current.ObjectType, "move");
        }


        /// <summary>
        /// 
        /// </summary>
        private Func<string, string> _getUpdateLRForMoveSqlBySqlServer;

        private string GetUpdateLRForMoveSqlBySqlServer(string tableName)
        {
            var idType = SQLServer.Util.GetSqlDbTypeString(this.IdProperty.PropertyType);

            //这里获取lft,rgt的值，并且更新子节点的lft,rgt数据
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("declare @rootId {0}; --根编号", idType);
            sql.AppendLine();
            sql.AppendLine("declare @lft int; --自身的左值");
            sql.AppendLine("declare @rgt int; --自身的右值");
            sql.AppendFormat("select @lft = lft,@rgt = rgt,@rootId=rootId from dbo.[{0}] where id=@currentId;", tableName); //先得到根编号
            sql.AppendLine();
            sql.AppendFormat("select id from dbo.[{0}] with(xlock,holdlock) where rootId=@rootId; --锁整个树", tableName);
            sql.AppendLine();

            sql.AppendLine("declare @disValue int; --需要更新的值");
            sql.AppendLine("set @disValue=@rgt-@lft+1;--牵连节点需要更新的值为 右值-左值+1");

            FillUpdateLRForMoveFirstSetpBeforeSql(sql);

            //1.先将子树设置为moveing状态
            sql.AppendFormat("update dbo.[{0}] set moving=1 where lft>@lft and lft<@rgt and rootId=@rootId;", tableName);
            sql.AppendLine();

            //1.先删除当前节点
            sql.AppendFormat("update dbo.[{0}] set rgt=rgt-@disValue where rgt>=@rgt and rootId=@rootId and moving=0;", tableName);  //moving=0，子树不受影响
            sql.AppendLine();
            sql.AppendFormat("update dbo.[{0}] set lft=lft-@disValue where lft>=@rgt and rootId=@rootId and moving=0;", tableName);
            sql.AppendLine();
            //2.插入当前节点到target
            sql.AppendLine("declare @prgt int; --父节点的右值");
            sql.AppendFormat("select @prgt = rgt from dbo.[{0}] where id=@targetId;", tableName);
            sql.AppendLine();
            sql.AppendLine("if(@prgt is null)");
            sql.AppendLine("begin");
            sql.AppendLine("	set @prgt=0;");
            sql.AppendLine("end");
            sql.AppendLine("else");
            sql.AppendLine("begin");
            sql.AppendFormat("	update dbo.[{0}] set rgt=rgt+@disValue where rgt>=@prgt and rootId=@rootId and moving=0;", tableName);//moving=0，子树不受影响
            sql.AppendLine();
            sql.AppendFormat("	update dbo.[{0}] set lft=lft+@disValue where lft>=@prgt and rootId=@rootId and moving=0;", tableName);
            sql.AppendLine();
            sql.AppendLine("end");


            //更新自己
            sql.AppendLine("declare @newLft int; --自身新的左值");
            sql.AppendLine("set @newLft=@prgt; --自身新的左值");

            sql.AppendFormat("update dbo.[{0}] set lft=@newLft,rgt=@newLft+@disValue-1,rootId=@rootId where id=@currentId;", tableName);
            sql.AppendLine();

            //更新子树
            sql.AppendLine("declare @lftDisValue int; --左值增量");
            sql.AppendLine("set @lftDisValue=@newLft-@lft;--子树立的节点需要更新的值为@lftDisValue");
            //更新左右值，并且恢复子树状态
            sql.AppendFormat("update dbo.[{0}] set lft=lft+@lftDisValue,rgt=rgt+@lftDisValue,moving=0 where lft > @lft and lft < @rgt and rootId=@rootId and moving=1;", tableName); //moving=1表示仅子树受影响
            sql.AppendLine();

            FillUpdateLRForMoveLastSetpAfterSql(sql);

            return sql.ToString();
        }

        /// <summary>
        /// 移动前要执行的sql
        /// </summary>
        /// <param name="sql"></param>
        protected virtual void FillUpdateLRForMoveFirstSetpBeforeSql(StringBuilder sql)
        {

        }

        /// <summary>
        /// 移动完成后执行的sql
        /// </summary>
        /// <param name="sq"></param>
        protected virtual void FillUpdateLRForMoveLastSetpAfterSql(StringBuilder sq)
        {

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
            sb.AppendFormat("select @lft = lft,@rgt=rgt,@rootId=rootId from dbo.[{0}] where id=@id;", tableName);
            sb.AppendLine();
            sb.AppendFormat("select {0} from dbo.[{1}] where lft<@lft and rgt>@rgt and rootId=@rootId order by lft;", KeyFields, tableName);
            return sb.ToString();
        }

        #endregion

    }
}
