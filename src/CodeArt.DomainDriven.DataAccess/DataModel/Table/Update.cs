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


namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        internal void Update(DomainObject obj)
        {
            if (obj == null || obj.IsEmpty() || !obj.IsDirty) return;

            DomainObject root = null;
            if (this.Type == DataTableType.AggregateRoot) root = obj;
            if (root == null || root.IsEmpty())
                throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, obj.ObjectType.FullName));

            CheckDataVersion(root);
            if (UpdateData(root, null, obj))
            {
                OnDataUpdate(root, obj);
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

                this.Mapper.FillUpdateData(obj, data);

                if (data.Count > 0)
                {
                    if (this.Type != DataTableType.AggregateRoot)
                    {
                        //补充根键
                        data.Add(GeneratedField.RootIdName, GetObjectId(root));
                    }

                    //补充主键
                    data.Add(EntityObject.IdPropertyName, GetObjectId(obj));

                    var sql = GetUpdateSql(data);
                    SqlHelper.Execute(this.Name, sql, data);

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

        /// <summary>
        /// 该方法用于修改数据后，更新基表的信息
        /// </summary>
        /// <param name="root"></param>
        /// <param name="obj"></param>
        private void OnDataUpdate(DomainObject root, DomainObject obj)
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

                //更新版本号
                SqlHelper.Execute(target.Name, target.SqlUpdateVersion, data);

                //更新代理对象的版本号
                var dataVersion = target.Type == DataTableType.AggregateRoot
                                    ? this.GetDataVersion(id)
                                    : this.GetDataVersion(GetObjectId(root), id);

                obj.DataProxy.Version = dataVersion;
            }
            this.Mapper.OnUpdate(obj);
        }


        private void UpdateMember(DomainObject root, DomainObject parent, DomainObject obj)
        {
            if (obj == null || obj.IsEmpty() || !obj.IsDirty) return;
            if (UpdateData(root, parent, obj))
            {
                OnDataUpdate(root, obj);
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
                            var value = current.GetValue(tip.Property);
                            data.Add(tip.PropertyName, GetValueListData(value));
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
                case DomainPropertyType.EntityObjectList:
                    {
                        if (current.IsPropertyChanged(tip.Property))
                        {
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
            }
            return false;
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
            foreach (DomainObject obj in objs)
            {
                if(!obj.IsEmpty())
                {
                    var child = GetRuntimeTable(this, tip.PropertyName, obj.ObjectType);
                    //方法内部会检查是否为脏，为脏的才更新
                    child.UpdateMember(root, current, obj);
                }
            }
        }


        private string GetUpdateSql(DynamicData data)
        {
            var query = UpdateTable.Create(this);
            return query.Build(data);
        }

        private string _sqlUpdateVersion = null;
        public string SqlUpdateVersion
        {
            get
            {
                if (_sqlUpdateVersion == null)
                {
                    _sqlUpdateVersion = GetUpdateVersionSql();
                }
                return _sqlUpdateVersion;
            }
        }

        private string GetUpdateVersionSql()
        {
            var query = UpdateDataVersion.Create(this);
            return query.Build(null);
        }
    }
}
