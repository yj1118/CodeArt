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

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        internal void Delete(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            if (this.Type == DataTableType.AggregateRoot)
            {
                DeleteRoot(obj);
            }
            else if (this.Type == DataTableType.EntityObjectPro)
            {
                var eop = obj as IEntityObjectPro;
                var root = eop?.Root;

                if (root == null || root.IsEmpty())
                    throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, obj.ObjectType.FullName));

                DeleteEntityObjectPro(root, eop);
            }
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
                    inherited.ExecuteDeleteData(this.TableIdName, rootId, id);
                }
                ExecuteDeleteData(this.TableIdName, rootId, id);
                return true;
            }
            else if (this.Type == DataTableType.EntityObjectPro)
            {
                foreach (var inherited in this.Inheriteds)
                {
                    inherited.ExecuteDeleteData(this.Root.TableIdName, rootId, id);
                }
                ExecuteDeleteData(this.Root.TableIdName, rootId, id);
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
                        inherited.ExecuteDeleteData(this.Root.TableIdName, rootId, id);
                    }
                    ExecuteDeleteData(this.Root.TableIdName, rootId, id);

                    return true;
                }
            }
        }

        /// <summary>
        /// 该方法用于删除数据后的补充操作
        /// </summary>
        /// <param name="obj"></param>
        private void OnDataDelete(object rootId, object id, DomainObject obj)
        {
            //删除缓存
            if (this.Type == DataTableType.AggregateRoot)
            {
                Cache.Remove(this.ObjectTip, rootId);
            }
            else
            {
                Cache.Remove(this.ObjectTip, rootId, id);
            }
        }

        private void ExecuteDeleteData(string rootIdName, object rootId, object id)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                data.Add(rootIdName, rootId);
                data.Add(EntityObject.IdPropertyName, id);
                SqlHelper.Execute(this.Name, this.SqlDelete, data);
            }
        }



        #region 删除根对象

        private void DeleteRoot(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            var ar = obj as IAggregateRoot;
            if (ar == null) throw new DataAccessException(Strings.CanNotDeleteNonAggregateRoot);

            var rootId = ar.GetIdentity();

            DeleteData(rootId, rootId);
            OnDataDelete(rootId, rootId, obj);
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
            return query.Build(null);
        }

        #endregion

        #region 删除高级引用对象

        private void DeleteEntityObjectPro(IAggregateRoot root, IEntityObjectPro eop)
        {
            var obj = (DomainObject)eop;

            //删除对象引用的成员
            DeleteMembers((DomainObject)root, (DomainObject)root, obj);
            //删除引用了该对象的中间表数据
            DeleteQuotesBySlave(root, obj);

            //删除自身
            var rootId = root.GetIdentity();
            var id = eop.GetIdentity();

            DeleteData(rootId, id);
            OnDataDelete(rootId, id, obj);
        }

        /// <summary>
        /// 删除目标对象<paramref name="parent"/>的成员数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        private void DeleteMembers(DomainObject root, DomainObject parent, DomainObject current)
        {
            //AggregateRoot、EntityObjectPro，由程序员手工调用删除
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
                    case DomainPropertyType.AggregateRootList:
                    case DomainPropertyType.EntityObjectProList:
                        {
                            //仅删除引用关系（也就是中间表数据）
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
            DataTable middle = null;
            foreach (DomainObject obj in members)
            {
                var child = GetRuntimeTable(this, propertyName, obj.ObjectType);
                //删除对象的数据
                child.DeleteMember(root, current, obj);
                if (middle == null) middle = child.Middle;
            }
            //删除相关中间表的数据（成员对象未必被真的删除，但是引用关系必须被删除，这样对于parent而言，引用关系被删除了）
            if(middle != null) middle.DeleteMiddleByMaster(root, current);
        }

        /// <summary>
        /// 删除引用了<paramref name="slave"/>数据的中间表数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="slave"></param>
        private void DeleteQuotesBySlave(IAggregateRoot root, DomainObject slave)
        {
            var quotes = GetQuoteMiddlesBySlave(this);
            foreach(var quote in quotes)
            {
                quote.DeleteMiddleBySlave((DomainObject)root, slave);
            }
        }

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

        #endregion

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


        #region 删除成员

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

            if (DeleteData(rootId, id))
            {
                DeleteMembers(root, parent, obj);
                OnDataDelete(rootId, id, obj);
            }
        }

        #endregion

        #region 删除中间表数据

        private void DeleteMiddleByMaster(DomainObject root, DomainObject master)
        {
            DeleteMiddle(root, master, null);
        }

        private void DeleteMiddleBySlave(DomainObject root, DomainObject slave)
        {
            DeleteMiddle(root, null, slave);
        }

        private void DeleteMiddle(DomainObject root, DomainObject master, DomainObject slave)
        {
            //中间表直接删除自身的数据
            var rootId = GetObjectId(root);
            var slaveIdName = this.SlaveField.Name;
            var slaveId = slave == null ? null : GetObjectId(slave);

            //if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            {
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(this.Root.TableIdName, rootId);
                    data.Add(slaveIdName, slaveId);
                    SqlHelper.Execute(this.ConnectionName, this.SqlDelete, data);
                }
            }
            else
            {
                var masterIdName = this.Master.TableIdName;
                var masterId = master == null ? null : GetObjectId(master);
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    data.Add(this.Root.TableIdName, rootId);
                    data.Add(masterIdName, masterId);
                    data.Add(slaveIdName, slaveId);
                    SqlHelper.Execute(this.ConnectionName, this.SqlDelete, data);
                }
            }
        }

        #endregion

    }
}