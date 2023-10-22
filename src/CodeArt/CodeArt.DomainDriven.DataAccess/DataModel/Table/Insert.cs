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
using CodeArt.AppSetting;
using CodeArt.DTO;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        internal void Insert(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;
            if(this.IsSnapshot)
            {
                //如果是镜像，不为空就可以保存了
            }
            else if(!obj.IsDirty)
            {
                //非镜像对象，要考虑是否为脏对象，脏对象才有保存的必要
                return;
            }

            DomainObject root = null;
            if (this.Type == DataTableType.AggregateRoot) root = obj;
            if (root == null || root.IsEmpty())
                throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, obj.ObjectType.FullName));

            OnPreDataInsert(obj);
            var data = InsertData(root, null, obj);
            OnDataInserted(root, obj, data);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal DynamicData InsertData(DomainObject root, DomainObject parent, DomainObject obj)
        {
            var data = GetInsertData(root, parent, obj);
            SqlHelper.Execute(this.SqlInsert, data);

            //如果有基表，那么继续插入
            var baseTable = this.BaseTable;
            if (baseTable != null)
            {
                var baseData = baseTable.InsertData(root, parent, obj);
                baseData.Combine(data);
                data = baseData;
            }
            return data;
        }

        private void OnPreDataInsert(DomainObject obj)
        {
            this.Mapper.OnPreInsert(obj, this);
        }


        /// <summary>
        /// 该方法用于保存数据后，更新基表的信息
        /// </summary>
        /// <param name="root"></param>
        /// <param name="obj"></param>
        private void OnDataInserted(DomainObject root, DomainObject obj, DynamicData objData)
        {
            if(!this.IsSnapshot)
            {   
                //当不是镜像时，需要改变状态，如果是镜像的保存，就不比更改原始对象的状态
                SetDataProxy(obj, objData, false);//对于保存的对象，我们依然要同步数据代理
                obj.MarkClean();
            }

            if (this.Type == DataTableType.AggregateRoot)
            {
                var ar = (IAggregateRoot)obj;
                DomainBuffer.Public.Add(this.ObjectTip.ObjectType, ar.GetIdentity(), ar);
            }

            if (this.IsDerived || this.IsDynamic)
            {
                //如果是派生对象或者动态对象，那么我们需要更新基表信息
                var inheritedRoot = this.InheritedRoot;
                using (var temp = SqlHelper.BorrowData())
                {
                    var data = temp.Item;
                    if (this.Type != DataTableType.AggregateRoot)
                    {
                        data.Add(GeneratedField.RootIdName, GetObjectId(root));
                    }

                    data.Add(EntityObject.IdPropertyName, GetObjectId(obj));

                    //修改类型码
                    var typeKey = this.IsDerived ? this.DerivedClass.TypeKey : this.DynamicType.Define.TypeName;
                    data.Add(GeneratedField.TypeKeyName, typeKey);

                    if(inheritedRoot.IsSessionEnabledMultiTenancy)
                    {
                        data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                    }

                    //更改基表的信息
                    var sql = inheritedRoot.GetUpdateSql(data);
                    SqlHelper.Execute(sql, data);
                }
            }

            this.Mapper.OnInserted(obj, this);
        }

        private DynamicData GetInsertData(DomainObject root, DomainObject parent, DomainObject obj)
        {
            Type objectType = this.ObjectType;

            var tips = Util.GetPropertyTips(objectType);
            var data = new DynamicData(); //由于对象会被缓存，因此不从池中获取DynamicData
            foreach (var tip in tips)
            {
                InsertAndCollectValue(root, parent, obj, tip, data);
            }

            if (this.Type == DataTableType.ValueObject)
            {
                //需要补充编号
                data.Add(EntityObject.IdPropertyName, GetObjectId(obj));
                //插入时默认为1
                data.Add(GeneratedField.AssociatedCountName, 1);
            }

            if (this.Type == DataTableType.EntityObject)
            {
                //插入时默认为1
                data.Add(GeneratedField.AssociatedCountName, 1);
            }

            if (this.Type == DataTableType.AggregateRoot)
            {
                if (this.IsSnapshot)
                {
                    data.Add(Util.SnapshotTime, DateTime.Now);
                    data.Add(Util.SnapshotLifespan, this.ObjectTip.SnapshotLifespan);
                }
            }
            else
            {
                //补充外键
                data.Add(GeneratedField.RootIdName, GetObjectId(root));
            }

            //this.Mapper.FillInsertData(obj, data, this);


            if (!this.IsDerived)
            {
                //只有非派生表才记录TypeKey和DataVersion
                data.Add(GeneratedField.TypeKeyName, string.Empty); //追加类型编号，非派生类默认类型编号为空
                data.Add(GeneratedField.DataVersionName, 1); //追加数据版本号
            }

            if(this.IsSessionEnabledMultiTenancy)
            {
                data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
            }

            return data;
        }

        /// <summary>
        /// 插入成员数据
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        /// <returns>成员有可能已经在别的引用中被插入，此时返回false,否则返回true</returns>
        private void InsertMember(DomainObject root, DomainObject parent, DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            //我们需要先查，看数据库中是否存在数据，如果不存在就新增，存在就增加引用次数
            var existObject = QuerySingle(GetObjectId(root), GetObjectId(obj));

            if (existObject.IsNull())
            {
                OnPreDataInsert(obj);
                var data = InsertData(root, parent, obj);
                OnDataInserted(root, obj, data);
            }
            else
            {
                if (this.IsDerived)
                {
                    this.InheritedRoot.IncrementAssociated(GetObjectId(root), GetObjectId(obj));
                }
                else
                {   
                    //递增引用次数
                    IncrementAssociated(GetObjectId(root), GetObjectId(obj));
                }
            }
        }


        private void InsertMiddle(IDomainObject root, IDomainObject master, IEnumerable slaves)
        {
            if(this.IsPrimitiveValue)
            {
                InsertMiddleByValues(root, master, slaves);
                return;
            }

            var rootId = GetObjectId(root);
            var rootIdName = GeneratedField.RootIdName;
            var slaveIdName = GeneratedField.SlaveIdName;

            if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            {
                int index = 0;
                foreach (var slave in slaves)
                {
                    if (slave.IsNull()) continue;
                    var slaveId = GetObjectId(slave);
                    using (var temp = SqlHelper.BorrowData())
                    {
                        var data = temp.Item;
                        data.Add(rootIdName, rootId);
                        data.Add(slaveIdName, slaveId);
                        data.Add(GeneratedField.OrderIndexName, index);
                        if (this.IsSessionEnabledMultiTenancy)
                            data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                        SqlHelper.Execute(this.SqlInsert, data);
                        index++;
                    }
                }
            }
            else
            {
                var masterIdName = GeneratedField.MasterIdName;
                var masterId = GetObjectId(master);
                int index = 0;
                foreach (var slave in slaves)
                {
                    if (slave.IsNull()) continue;
                    var slaveId = GetObjectId(slave);
                    using (var temp = SqlHelper.BorrowData())
                    {
                        var data = temp.Item;
                        data.Add(rootIdName, rootId);
                        data.Add(masterIdName, masterId);
                        data.Add(slaveIdName, slaveId);
                        data.Add(GeneratedField.OrderIndexName, index);
                        if (this.IsSessionEnabledMultiTenancy)
                            data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                        SqlHelper.Execute(this.SqlInsert, data);
                        index++;
                    }
                }

            }
        }

        private void InsertMiddleByValues(IDomainObject root, IDomainObject master, IEnumerable values)
        {
            var rootId = GetObjectId(root);
            var rootIdName = GeneratedField.RootIdName;
            if (this.Root.IsEqualsOrDerivedOrInherited(this.Master))
            {
                int index = 0;
                foreach (var value in values)
                {
                    using (var temp = SqlHelper.BorrowData())
                    {
                        var data = temp.Item;
                        data.Add(rootIdName, rootId);
                        data.Add(GeneratedField.PrimitiveValueName, value);
                        data.Add(GeneratedField.OrderIndexName, index);
                        if(this.IsSessionEnabledMultiTenancy)
                            data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                        SqlHelper.Execute(this.SqlInsert, data);
                        index++;
                    }
                }
            }
            else
            {
                var masterIdName = GeneratedField.MasterIdName;
                var masterId = GetObjectId(master);
                int index = 0;
                foreach (var value in values)
                {
                    using (var temp = SqlHelper.BorrowData())
                    {
                        var data = temp.Item;
                        data.Add(rootIdName, rootId);
                        data.Add(masterIdName, masterId);
                        data.Add(GeneratedField.PrimitiveValueName, value);
                        data.Add(GeneratedField.OrderIndexName, index);
                        if (this.IsSessionEnabledMultiTenancy)
                            data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                        SqlHelper.Execute(this.SqlInsert, data);
                        index++;
                    }
                }
            }
        }

        private void InsertAndCollectValue(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip, DynamicData data)
        {
            if (this.IsSnapshot && !tip.Snapshot) return; //如果是快照，那么需要过滤不参与快照的属性

            //if (tip.TrySaveData(this.ObjectType, current, data)) return;

            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.Primitive:
                    {
                        var value = GetPrimitivePropertyValue(current, tip);
                        data.Add(tip.PropertyName, value);
                    }
                    break;
                case DomainPropertyType.PrimitiveList:
                    {
                        var value = current.GetValue(tip.Property);
                        //仅存中间表
                        var values = GetValueListData(value);
                        var child = GetChildTableByRuntime(this, tip);//无论是派生还是基类，基础表对应的中间表都一样
                        child.InsertMiddle(root, current, values);
                    }
                    break;
                case DomainPropertyType.ValueObject:
                    {
                        InsertAndCollectValueObject(root, parent, current, tip, data);
                    }
                    break;
                case DomainPropertyType.AggregateRoot:
                    {
                        var field = GetQuoteField(this, tip.PropertyName);
                        object obj = current.GetValue(tip.Property);
                        var id = GetObjectId(obj);
                        data.Add(field.Name, id);
                    }
                    break;
                case DomainPropertyType.EntityObject:
                    {
                        var obj = current.GetValue(tip.Property) as DomainObject;

                        var id = GetObjectId(obj);
                        var field = GetQuoteField(this, tip.PropertyName);
                        data.Add(field.Name, id);  //收集外键

                        //保存引用数据
                        if (!obj.IsEmpty())
                        {
                            var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                            child.InsertMember(root, current, obj);
                        }
                    }
                    break;
                case DomainPropertyType.AggregateRootList:
                    {
                        //仅存中间表
                        var objs = current.GetValue(tip.Property) as IEnumerable;
                        var child = GetChildTableByRuntime(this, tip);//无论是派生还是基类，基础表对应的中间表都一样
                        child.Middle.InsertMiddle(root, current, objs);
                    }
                    break;
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.EntityObjectList:
                    {
                        InsertMembers(root, parent, current, tip);
                    }
                    break;
            }
        }

        private void InsertAndCollectValueObject(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip, DynamicData data)
        {
            var field = GetQuoteField(this, tip.PropertyName);
            var obj = current.GetValue(tip.Property) as DomainObject;

            if (obj.IsEmpty())
            {
                data.Add(field.Name, Guid.Empty);
            }
            else
            {
                (obj as IValueObject).TrySetId(Guid.NewGuid());
                var id = GetObjectId(obj);
                data.Add(field.Name, id);

                //保存数据
                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                child.InsertMember(root, current, obj);
            }
        }

        private void InsertMembers(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var objs = current.GetValue(tip.Property) as IEnumerable;
            InsertMembers(root, parent, current, objs, tip);
        }

        private void InsertMembers(DomainObject root, DomainObject parent, DomainObject current, IEnumerable members, PropertyRepositoryAttribute tip)
        {
            DataTable middle = null;
            foreach (DomainObject obj in members)
            {
                if (obj.IsEmpty()) continue;
                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                if (child.Type == DataTableType.ValueObject)
                {
                    //我们需要为ValueObject补充编号
                    (obj as IValueObject).TrySetId(Guid.NewGuid());
                }
                child.InsertMember(root, current, obj);
                if (middle == null) middle = child.Middle;
            }
            if (middle != null) middle.InsertMiddle(root, current, members);
        }


        private string _sqlInsert = null;
        public string SqlInsert
        {
            get
            {
                if (_sqlInsert == null)
                {
                    _sqlInsert = GetInsertSql();
                }
                return _sqlInsert;
            }
        }

        private string GetInsertSql()
        {
            var query = InsertTable.Create(this);
            return query.Build(null, this);
        }

    }

}
