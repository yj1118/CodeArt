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
            if (obj == null || obj.IsEmpty() || !obj.IsDirty) return;

            DomainObject root = null;
            if (this.Type == DataTableType.AggregateRoot) root = obj;
            else if (this.Type == DataTableType.EntityObjectPro)
            {
                var eop = obj as IEntityObjectPro;
                root = eop?.Root as DomainObject;
            }
            if (root == null || root.IsEmpty())
                throw new DomainDrivenException(Strings.PersistentObjectError);

            var data = InsertData(root, null, obj);
            OnDataInsert(root, obj, data);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal DynamicData InsertData(DomainObject root, DomainObject parent, DomainObject obj)
        {
            var data = GetInsertData(root, parent, obj);
            SqlHelper.Execute(this.ConnectionName, this.SqlInsert, data);

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

        /// <summary>
        /// 该方法用于保存数据后，更新基表的信息
        /// </summary>
        /// <param name="root"></param>
        /// <param name="obj"></param>
        private void OnDataInsert(DomainObject root, DomainObject obj, DynamicData objData)
        {
            SetDataProxy(obj, objData);//对于保存的对象，我们依然要同步数据代理
            obj.MarkClean();

            if (this.Type == DataTableType.AggregateRoot)
            {
                Cache.Add(this.ObjectTip, GetObjectId(obj), obj);
            }
            else
            {
                Cache.Add(this.ObjectTip, GetObjectId(root), GetObjectId(obj), obj);
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
                        data.Add(this.Root.TableIdName, GetObjectId(root));
                    }

                    data.Add(EntityObject.IdPropertyName, GetObjectId(obj));

                    //修改类型码
                    var typeKey = this.IsDerived ? this.DerivedClass.TypeKey : this.DynamicType.Define.TypeName;
                    data.Add(GeneratedField.TypeKeyName, typeKey);

                    //更改基表的信息
                    var sql = inheritedRoot.GetUpdateSql(data);
                    SqlHelper.Execute(inheritedRoot.Name, sql, data);
                }
            }
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
                if(this.Type != DataTableType.EntityObjectPro) //EntityObjectPro不用补充外键
                {
                    //补充外键
                    data.Add(this.Root.TableIdName, GetObjectId(root));
                }
            }

            if(!this.IsDerived)
            {
                //只有非派生表才记录TypeKey和DataVersion
                data.Add(GeneratedField.TypeKeyName, string.Empty); //追加类型编号，非派生类默认类型编号为空
                data.Add(GeneratedField.DataVersionName, 1); //追加数据版本号
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
                var data = InsertData(root, parent, obj);
                OnDataInsert(root, obj, data);
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
            var rootId = GetObjectId(root);
            var rootIdName = this.Root.TableIdName;
            var slaveIdName = this.SlaveField.Name;

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
                        data.Add(GeneratedField.OrderIndexName , index);
                        SqlHelper.Execute(this.ConnectionName, this.SqlInsert, data);
                        index++;
                    }
                }
            }
            else
            {
                var masterIdName = this.Master.TableIdName;
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
                        SqlHelper.Execute(this.ConnectionName, this.SqlInsert, data);
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
                        data.Add(tip.PropertyName, GetValueListData(value));
                    }
                    break;
                case DomainPropertyType.ValueObject:
                    {
                        InsertAndCollectValueObject(root, parent, current, tip, data);
                    }
                    break;
                case DomainPropertyType.EntityObjectPro:
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
                case DomainPropertyType.EntityObjectProList:
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
            var obj = current.GetValue(tip.Property) as ValueObject;

            if (obj.IsEmpty())
            {
                data.Add(field.Name, Guid.Empty);
            }
            else
            {
                obj.SetId(Guid.NewGuid());
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
            DataTable middle = null;
            foreach (DomainObject obj in objs)
            {
                if (obj.IsEmpty()) continue;
                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                if (child.Type == DataTableType.ValueObject)
                {
                    //我们需要为ValueObject补充编号
                    (obj as ValueObject).SetId(Guid.NewGuid());
                }
                child.InsertMember(root, current, obj);
                if (middle == null) middle = child.Middle;
            }
            if (middle != null) middle.InsertMiddle(root, current, objs);
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
            return query.Build(null);
        }

    }

}
