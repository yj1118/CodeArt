using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Collections;

using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        internal void Update(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty()) return;

            DomainObject root = null;
            if (this.Type == DataTableType.AggregateRoot) root = obj;
            if (root == null || root.IsEmpty())
                throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, obj.ObjectType.FullName));

            if (obj.IsDirty)
            {
                CheckDataVersion(root);
                OnPreDataUpdate(obj);
                if (UpdateData(root, null, obj))
                {
                    OnDataUpdated(root, obj);
                }
            }
            else
            {
                //如果对象不是脏的，但是要求修改，那么有可能是该对象的引用链上的对象发生了变化，所以我们移除该对象的缓冲
                DomainBuffer.Public.Remove(obj.ObjectType, GetObjectId(obj));
            }
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal bool UpdateData(DomainObject root, DomainObject parent, DomainObject obj)
        {
            bool isChanged = false;

            var tips = Util.GetPropertyTips(this.ObjectType);
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                foreach (var tip in tips)
                {
                    var memberIsChanged = UpdateAndCollectChangedValue(root, parent, obj, tip, data);
                    if (!isChanged) isChanged = memberIsChanged;
                }

                //this.Mapper.FillUpdateData(obj, data, this);

                if (data.Count > 0)
                {
                    if (this.Type != DataTableType.AggregateRoot)
                    {
                        //补充根键
                        data.Add(GeneratedField.RootIdName, GetObjectId(root));
                    }

                    //补充主键
                    data.Add(EntityObject.IdPropertyName, GetObjectId(obj));

                    if(this.IsSessionEnabledMultiTenancy)
                    {
                        data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                    }

                    var sql = GetUpdateSql(data);
                    SqlHelper.Execute(sql, data);

                    //更新代理对象中的数据
                    (obj.DataProxy as DataProxyPro).OriginalData.Update(data);
                }
            }

            //如果有基表，那么继续修改
            var baseTable = this.BaseTable;
            if (baseTable != null)
            {
                var baseIsChanged = baseTable.UpdateData(root, parent, obj);
                if (!isChanged) isChanged = baseIsChanged;
            }
            return isChanged;
        }

        private void OnPreDataUpdate(DomainObject obj)
        {
            this.Mapper.OnPreUpdate(obj, this);
        }

        /// <summary>
        /// 该方法用于修改数据后，更新基表的信息
        /// </summary>
        /// <param name="root"></param>
        /// <param name="obj"></param>
        private void OnDataUpdated(DomainObject root, DomainObject obj)
        {
            obj.MarkClean(); //修改之后，就干净了

            var id = GetObjectId(obj);

            //更新数据版本号
            var target = this.IsDerived ? this.InheritedRoot : this;
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                if (target.Type != DataTableType.AggregateRoot)
                {
                    data.Add(GeneratedField.RootIdName, GetObjectId(root));
                }

                data.Add(EntityObject.IdPropertyName, id);

                if (this.IsSessionEnabledMultiTenancy)
                {
                    data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                }

                //更新版本号
                SqlHelper.Execute(target.GetUpdateVersionSql(), data);

                //更新代理对象的版本号
                var dataVersion = target.Type == DataTableType.AggregateRoot
                                    ? this.GetDataVersion(id)
                                    : this.GetDataVersion(GetObjectId(root), id);

                obj.DataProxy.Version = dataVersion;
            }

            if (this.Type == DataTableType.AggregateRoot)
            {
                if (obj.IsMirror)
                {
                    //镜像被修改了，对应的公共缓冲区中的对象也要被重新加载
                    DomainBuffer.Public.Remove(obj.ObjectType, id);
                }
            }

            

            this.Mapper.OnUpdated(obj, this);
        }


        private void UpdateMember(DomainObject root, DomainObject parent, DomainObject obj)
        {
            if (obj == null || obj.IsEmpty() || !obj.IsDirty) return;
            if (UpdateData(root, parent, obj))
            {
                OnDataUpdated(root, obj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        /// <param name="data"></param>
        /// <returns>当内部成员发生变化，返回true</returns>
        private bool UpdateAndCollectChangedValue(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip, DynamicData data)
        {
            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.Primitive:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            var value = GetPrimitivePropertyValue(current, tip);
                            data.Add(tip.PropertyName, value);
                            return true;
                        }
                    }
                    break;
                case DomainPropertyType.PrimitiveList:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            //删除老数据
                            var child = GetChildTableByRuntime(this, tip);
                            child.DeleteMiddleByMaster(root, current);

                            var value = current.GetValue(tip.Property);
                            //仅存中间表
                            var values = GetValueListData(value);
                            child.InsertMiddle(root, current, values);
                            return true;
                        }
                    }
                    break;
                //case DomainPropertyType.ValueObject:
                //    {
                //        if (current.IsPropertyChanged(tip.Property))
                //        {
                //            //删除原始数据
                //            DeleteMemberByOriginalData(root, parent, current, tip);
                //            //新增数据
                //            InsertAndCollectValueObject(root, parent, current, tip, data);
                //            return true;
                //        }
                //    }
                //    break;
                case DomainPropertyType.AggregateRoot:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            var field = GetQuoteField(this, tip.PropertyName);
                            object obj = current.GetValue(tip.Property);
                            var id = GetObjectId(obj);
                            data.Add(field.Name, id);
                            return true;
                        }
                    }
                    break;
                case DomainPropertyType.ValueObject: //虽然值对象的成员不会变，但是成员的成员也许会改变
                case DomainPropertyType.EntityObject:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            var obj = current.GetValue(tip.Property) as DomainObject;
                            if(tip.DomainPropertyType == DomainPropertyType.ValueObject)
                            {
                                (obj as IValueObject).TrySetId(Guid.NewGuid());
                            }

                            var id = GetObjectId(obj);
                            var field = GetQuoteField(this, tip.PropertyName);
                            data.Add(field.Name, id);  //收集外键

                            //删除原始数据
                            DeleteMemberByOriginalData(root, parent, current, tip);

                            //保存引用数据
                            if (!obj.IsEmpty())
                            {
                                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                                child.InsertMember(root, current, obj);
                            }
                            return true;
                        }
                        else if (current.IsPropertyDirty(tip.Property))
                        {
                            //如果引用的内聚成员是脏对象，那么需要修改
                            var obj = current.GetValue(tip.Property) as DomainObject;
                            if (!obj.IsEmpty())
                            {
                                //从衍生表中找到对象表
                                var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                                child.UpdateMember(root, parent, obj);
                            }
                            return true;
                        }
                    }
                    break;
                case DomainPropertyType.AggregateRootList:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            //删除老数据
                            var child = GetChildTableByRuntime(this, tip);
                            child.Middle.DeleteMiddleByMaster(root, current);

                            //追加新数据
                            var objs = current.GetValue(tip.Property) as IEnumerable;
                            child.Middle.InsertMiddle(root, current, objs);
                            return true;
                        }
                    }
                    break;
                case DomainPropertyType.ValueObjectList:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            //在删除数据之前，需要预读对象，确保子对象延迟加载的数据也被增加了，否则会引起数据丢失
                            PreRead(current);

                            //引用关系发生了变化，删除重新追加
                            //这里要注意，需要删除的是数据库的数据，所以要重新读取
                            //删除原始数据
                            DeleteMembersByOriginalData(root, parent, current, tip);

                            //加入新数据
                            InsertMembers(root, parent, current, tip);
                            return true;
                        }
                        else if (current.IsPropertyDirty(tip.Property))
                        {
                            //引用关系没变，只是数据脏了
                            UpdateMembers(root, parent, current, tip);
                            return true;
                        }
                    }
                    break;
                case DomainPropertyType.EntityObjectList:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
                            var targets = current.GetValue(tip.Property) as IEnumerable<IDomainObject>;

                            //加载原始数据
                            var rawData = ((DataProxyPro)current.DataProxy).OriginalData;
                            var raw = ReadMembers(current, tip, null, rawData, QueryLevel.None) as IEnumerable<IDomainObject>;
                            //对比
                            var diff = raw.Transform(targets);

                            
                            InsertMembers(root, parent, current, diff.Adds, tip);

                            DeleteMembers(root, parent, current, diff.Removes, tip);

                            UpdateMembers(root, parent, current, diff.Updates, tip);


                            //以上3行代码会打乱成员顺序，所以要更新下排序
                            UpdateOrderIndexs(root, parent, current, targets, tip);//更新排序

                            return true;
                        }
                        else if (current.IsPropertyDirty(tip.Property))
                        {
                            //引用关系没变，只是数据脏了
                            UpdateMembers(root, parent, current, tip);
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        private static void PreRead(DomainObject obj)
        {
            Type objectType = obj.ObjectType;

            var tips = Util.GetPropertyTips(objectType);
            foreach (var tip in tips)
            {
                PreReadProperty(obj, tip);
            }
        }

        private static object PreReadProperty(DomainObject current, PropertyRepositoryAttribute tip)
        {
            switch (tip.DomainPropertyType)
            {
                case DomainPropertyType.Primitive:
                    {
                        return GetPrimitivePropertyValue(current, tip);
                    }
                case DomainPropertyType.PrimitiveList:
                    {
                        return current.GetValue(tip.Property);
                    }
                case DomainPropertyType.ValueObject:
                    {
                        var obj = current.GetValue(tip.Property) as DomainObject;
                        PreRead(obj);
                        return obj;
                    }
                case DomainPropertyType.AggregateRoot:
                    {
                        //仅获得引用即可，不需要完整的预读
                        return current.GetValue(tip.Property);
                    }
                case DomainPropertyType.EntityObject:
                    {
                        var obj = current.GetValue(tip.Property) as DomainObject;
                        PreRead(obj);
                        return obj;
                    }
                case DomainPropertyType.AggregateRootList:
                    {
                        //仅获得引用即可，不需要完整的预读
                        return current.GetValue(tip.Property);
                    }
                case DomainPropertyType.ValueObjectList:
                case DomainPropertyType.EntityObjectList:
                    {
                        var objs = current.GetValue(tip.Property) as IEnumerable;
                        foreach(DomainObject obj in objs)
                        {
                            PreRead(obj);
                        }
                        return objs;
                    }
            }
            return null;
        }


        /// <summary>
        /// 修改current对应的集合属性
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="current"></param>
        /// <param name="tip"></param>
        private void UpdateMembers(DomainObject root, DomainObject parent, DomainObject current, PropertyRepositoryAttribute tip)
        {
            var objs = current.GetValue(tip.Property) as IEnumerable;
            UpdateMembers(root, parent, current, objs, tip);
        }

        private void UpdateMembers(DomainObject root, DomainObject parent, DomainObject current, IEnumerable members, PropertyRepositoryAttribute tip)
        {
            foreach (DomainObject obj in members)
            {
                if (!obj.IsEmpty())
                {
                    var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                    //方法内部会检查是否为脏，为脏的才更新
                    child.UpdateMember(root, current, obj);
                }
            }
        }


        private void UpdateOrderIndexs(DomainObject root, DomainObject parent, DomainObject current, IEnumerable members, PropertyRepositoryAttribute tip)
        {
            //先删除，再添加
            var propertyName = tip.PropertyName;
            DataTable child = null;
            foreach (DomainObject obj in members)
            {
                if (obj.IsEmpty()) continue;
                if(child == null) child = GetRuntimeTable(this, propertyName, obj.ObjectType);
                //删除中间表
                child.Middle.DeleteMiddle(root, current, obj);
            }

            //重新添加中间表
            if(child != null)
                child.Middle.InsertMiddle(root, current, members);
        }

        //private void UpdateMiddle(IDomainObject root, IDomainObject master, IEnumerable slaves, PropertyRepositoryAttribute tip)
        //{
        //    this.DeleteMiddle()

        //    var rootId = GetObjectId(root);
        //    var rootIdName = GeneratedField.RootIdName;
        //    var slaveIdName = GeneratedField.SlaveIdName;

        //    if (master == null || this.Root.IsEqualsOrDerivedOrInherited(this.Master))
        //    {
        //        int index = 0;
        //        foreach (var slave in slaves)
        //        {
        //            if (slave.IsNull()) continue;
        //            var slaveId = GetObjectId(slave);
        //            using (var temp = SqlHelper.BorrowData())
        //            {
        //                var data = temp.Item;
        //                data.Add(rootIdName, rootId);
        //                data.Add(slaveIdName, slaveId);
        //                data.Add(GeneratedField.OrderIndexName, index);
        //                if (this.IsEnabledMultiTenancy)
        //                    data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
        //                SqlHelper.Execute(this.GetUpdateSql(data), data);
        //                index++;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var masterIdName = GeneratedField.MasterIdName;
        //        var masterId = GetObjectId(master);
        //        int index = 0;
        //        foreach (var slave in slaves)
        //        {
        //            if (slave.IsNull()) continue;
        //            var slaveId = GetObjectId(slave);
        //            using (var temp = SqlHelper.BorrowData())
        //            {
        //                var data = temp.Item;
        //                data.Add(rootIdName, rootId);
        //                data.Add(masterIdName, masterId);
        //                data.Add(slaveIdName, slaveId);
        //                data.Add(GeneratedField.OrderIndexName, index);
        //                if (this.IsEnabledMultiTenancy)
        //                    data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
        //                SqlHelper.Execute(this.GetUpdateSql(data), data);
        //                index++;
        //            }
        //        }

        //    }
        //}


        private string GetUpdateSql(DynamicData data)
        {
            var query = UpdateTable.Create(this, this.IsSessionEnabledMultiTenancy);
            return query.Build(data, this);
        }

        private string GetUpdateVersionSql()
        {
            var query = UpdateDataVersion.Create(this, this.IsSessionEnabledMultiTenancy); //不能直接用table检索，因为环境不同，IsEnabledMultiTenancy会有变化
            return query.Build(null, this);
        }
    }
}
