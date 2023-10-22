using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Collections;

using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        internal void Delete(DomainObject obj)
        {
            if (this.Type == DataTableType.AggregateRoot)
            {
                DeleteRoot(obj);
                return;
            }

            throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, obj.ObjectType.FullName));
        }

        ///// <summary>
        ///// 删除数据
        ///// </summary>
        ///// <param name="root"></param>
        ///// <param name="parent"></param>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        private bool DeleteData(object rootId, object id)
        {
            if (this.Type == DataTableType.AggregateRoot)
            {
                foreach (var inherited in this.Inheriteds)
                {
                    inherited.ExecuteDeleteData(rootId, id);
                }
                ExecuteDeleteData(rootId, id);
                return true;
            }
            else
            {
                //删除成员表，成员表会记录引用次数，以此来判定是否真实删除
                var table = this.IsDerived ? this.InheritedRoot : this;

                var associated = table.GetAssociated(rootId, id);
                if (associated > 1)
                {
                    table.DecrementAssociated(rootId, id);
                    return false;
                }
                else
                {
                    foreach (var inherited in this.Inheriteds)
                    {
                        inherited.ExecuteDeleteData(rootId, id);
                    }
                    ExecuteDeleteData(rootId, id);

                    return true;
                }
            }
        }

        private void OnPreDataDelete(DomainObject obj)
        {
            this.Mapper.OnPreDelete(obj, this);
        }

        /// <summary>
        /// 该方法用于删除数据后的补充操作
        /// </summary>
        /// <param name="obj"></param>
        private void OnDataDeleted(object rootId, object id, DomainObject obj)
        {
            //从缓冲区中删除对象
            if (this.Type == DataTableType.AggregateRoot)
            {
                DomainBuffer.Public.Remove(this.ObjectTip.ObjectType, rootId); //不用考虑mirror，删不删除都不影响
            }
            this.Mapper.OnDeleted(obj, this);
        }

        private void ExecuteDeleteData(object rootId, object id)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                data.Add(GeneratedField.RootIdName, rootId);
                data.Add(EntityObject.IdPropertyName, id);
                AddToTenant(data);

                SqlHelper.Execute(this.SqlDelete, data);
            }
        }



        #region 删除根对象

        private void DeleteRoot(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            var ar = obj as IAggregateRoot;
            if (ar == null) throw new DataAccessException(Strings.CanNotDeleteNonAggregateRoot);

            CheckDataVersion(obj);

            var rootId = ar.GetIdentity();
            OnPreDataDelete(obj);
            if(DeleteData(rootId, rootId))
            {
                DeleteMiddles(obj);
                DeleteMembers(obj, obj, obj);
                OnDataDeleted(rootId, rootId, obj);
            }
            
        }

        private void DeleteMiddles(DomainObject obj)
        {
            var middles = RootIsSlaveIndex.Get(this);
            foreach(var middle in middles)
            {
                middle.DeleteMiddleByRootSlave(obj);
            }
        }


        private string _sqlDelete = null;
        public string SqlDelete
        {
            get
            {
                if (_sqlDelete == null)
                {
                    _sqlDelete = GetDeleteSql();
                }
                return _sqlDelete;
            }
        }

        private string GetDeleteSql()
        {
            var query = DeleteTable.Create(this);
            return query.Build(null, this);
        }

        #endregion

        #region 删除成员

        /// <summary>
        /// 删除目标对象<paramref name="parent"/>的成员数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        private void DeleteMembers(DomainObject root, DomainObject parent, DomainObject current)
        {
            //AggregateRoot，由程序员手工调用删除
            var tips = Util.GetPropertyTips(current.GetType());
            foreach (var tip in tips)
            {
                switch (tip.DomainPropertyType)
                {
                    case DomainPropertyType.EntityObject:
                    case DomainPropertyType.ValueObject:
                        {
                            DeleteMemberByPropertyValue(root, parent, current, tip);
                        }
                        break;
                    case DomainPropertyType.EntityObjectList:
                    case DomainPropertyType.ValueObjectList:
                        {
                            DeleteMembersByPropertyValue(root, parent, current, tip);
                        }
                        break;
                    case DomainPropertyType.PrimitiveList:
                        {
                            //删除老数据
                            var child = GetChildTableByRuntime(this, tip);
                            child.DeleteMiddleByMaster(root, current);
                        }
                        break;
                    case DomainPropertyType.AggregateRootList:
                        {
                            //仅删除引用关系（也就是中间表数据），由于不需要删除slave根表的数据，因此直接使用该方法更高效，不需要读取实际集合值
                            DeleteQuotesByMaster(root, current);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 根据对象当前定义的属性，删除成员
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        private void DeleteMembersByPropertyValue(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var objs = current.GetValue(tip.Property) as IEnumerable;
            DeleteMembers(root, parent, current, objs, tip);
        }

        /// <summary>
        /// 从数据库中加载成员，并且删除
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        private void DeleteMembersByOriginalData(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var originalData = GetOriginalData(current);

            using (var temp = ListPool<object>.Borrow())
            {
                var objs = temp.Item;
                QueryMembers(tip, originalData, objs);
                DeleteMembers(root, parent, current, objs, tip);
            }
        }

        /// <summary>
        /// 该方法是一个重构类型的工具方法，删除<paramref name="current"/>上的属性数据，属性类型为对象的集合
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="members"></param>
        /// <param name="tip"></param>
        private void DeleteMembers(DomainObject root, DomainObject parent, DomainObject current, IEnumerable members, PropertyRepositoryAttribute tip)
        {
            var propertyName = tip.PropertyName;
            foreach (DomainObject obj in members)
            {
                if (obj.IsEmpty()) continue;
                var child = GetRuntimeTable(this, propertyName, obj.ObjectType);
                //删除对象的数据
                child.DeleteMember(root, current, obj);
                child.Middle.DeleteMiddle(root, current, obj);
            }
        }

        //由于两个原因，我们不会删除根对象后，去反向找哪些表的字段引用了该根对象，
        //1.领域有空对象的概念，引用的根对象没了后，加载的是空对象
        //2.不同的子系统物理分布都不一样，表都在不同的数据库里，没有办法也不需要去追踪
        //因此DeleteQuotesBySlave我们用不上
        ///// <summary>
        ///// 删除引用了<paramref name="slave"/>数据的中间表数据
        ///// </summary>
        ///// <param name="root"></param>
        ///// <param name="slave"></param>
        //private void DeleteQuotesBySlave(DomainObject root, DomainObject slave)
        //{
        //    var quotes = GetQuoteMiddlesBySlave(this);
        //    foreach (var quote in quotes)
        //    {
        //        quote.DeleteMiddleBySlave(root, slave);
        //    }
        //}

        /// <summary>
        /// 删除引用了<paramref name="master"/>数据的中间表数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="slave"></param>
        private void DeleteQuotesByMaster(DomainObject root, DomainObject master)
        {
            var quotes = GetQuoteMiddlesByMaster(this);
            foreach (var quote in quotes)
            {
                quote.DeleteMiddleByMaster(root, master);
            }
        }

        /// <summary>
        /// 根据当前属性值，删除成员
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        private void DeleteMemberByPropertyValue(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var obj = current.GetValue(tip.Property) as DomainObject;
            if (!obj.IsEmpty())
            {
                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                child.DeleteMember(root, parent, obj);
            }
        }


        /// <summary>
        /// 从数据库中加载成员，并且删除
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        private void DeleteMemberByOriginalData(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var originalData = GetOriginalData(current);
            var obj = QueryMember(tip, originalData) as DomainObject;
            if(!obj.IsEmpty())
            {
                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                child.DeleteMember(root, parent, obj);
            }
        }

        /// <summary>
        /// 删除成员（该方法用于删除 ValueObject,EntityObject等对象）
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        private void DeleteMember(DomainObject root, DomainObject parent, DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            var rootId = GetObjectId(root);
            var id = GetObjectId(obj);

            OnPreDataDelete(obj);
            if (DeleteData(rootId, id))
            {
                DeleteMembers(root, parent, obj);
                OnDataDeleted(rootId, id, obj);
            }
        }

        #endregion

        #region 删除中间表数据

        private void DeleteMiddleByMaster(DomainObject root, DomainObject master)
        {
            DeleteMiddle(root, master, null);
        }

        private void DeleteMiddleByRootSlave(DomainObject slave)
        {
            //根据slave删除中间表数据
            var slaveId = GetObjectId(slave);
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                data.Add(GeneratedField.RootIdName, null);
                data.Add(GeneratedField.MasterIdName, null);
                data.Add(GeneratedField.SlaveIdName, slaveId);
                AddToTenant(data);
                SqlHelper.Execute(this.SqlDelete, data);
            }
        }

        private void DeleteMiddle(DomainObject root, DomainObject master, DomainObject slave)
        {
            if(this.IsPrimitiveValue)
            {
                DeleteMiddleByValues(root, master);
                return;
            }

            //中间表直接删除自身的数据
            var rootId = GetObjectId(root);
            var slaveId = slave == null ? null : GetObjectId(slave);

            if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            {
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(GeneratedField.RootIdName, rootId);
                    data.Add(GeneratedField.SlaveIdName, slaveId);//slaveId有可能为空，因为是根据master删除，但是没有关系，sql会处理slaveId为空的情况
                    AddToTenant(data);
                    SqlHelper.Execute(this.SqlDelete, data);
                }
            }
            else
            {
                var masterId = master == null ? null : GetObjectId(master);
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(GeneratedField.RootIdName, rootId);
                    data.Add(GeneratedField.MasterIdName, masterId);
                    data.Add(GeneratedField.SlaveIdName, slaveId);
                    AddToTenant(data);
                    SqlHelper.Execute(this.SqlDelete, data);
                }
            }
        }

        private void DeleteMiddleByValues(DomainObject root, DomainObject master)
        {
            //中间表直接删除自身的数据
            var rootId = GetObjectId(root);

            if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            {
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(GeneratedField.RootIdName, rootId);
                    AddToTenant(data);
                    SqlHelper.Execute(this.SqlDelete, data);
                }
            }
            else
            {
                var masterId = master == null ? null : GetObjectId(master);
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(GeneratedField.RootIdName, rootId);
                    data.Add(GeneratedField.MasterIdName, masterId);
                    AddToTenant(data);
                    SqlHelper.Execute(this.SqlDelete, data);
                }
            }
        }


        #endregion

    }
}