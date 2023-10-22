using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;
using System.Collections;

using CodeArt.DTO;
using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Concurrent;
using System.Collections.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        /// <summary>
        /// 获得不带任何引用链的、独立、干净的表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetAbsolute()
        {
            switch(this.Type)
            {
                case DataTableType.AggregateRoot:
                    {
                        return DataModel.Create(this.ObjectType).Root;
                    }
                case DataTableType.EntityObject:
                case DataTableType.ValueObject:
                    {
                        return this; //对于成员类型，直接返回，因为没有干净的
                    }
            }
            throw new DataAccessException("未知的异常");
        }

        /// <summary>
        /// 获取基元类型的属性值
        /// </summary>
        /// <returns></returns>
        private static object GetPrimitivePropertyValue(DomainObject obj, PropertyRepositoryAttribute tip)
        {
            var value = obj.GetValue(tip.Property);
            if (!tip.IsEmptyable) return value;
            var e = (IEmptyable)value;
            return e.IsEmpty() ? null : e.GetValue(); //可以存null值在数据库
        }


        internal long GetIdentity()
        {
            var query = GetIncrementIdentity.Create(this);
            var sql = query.Build(null, this);

            //所有的租户在某个根对象上，获取自增的序列是一样的，这样方便聚合根的ID是全局唯一的
            return SqlHelper.ExecuteScalar<long>(sql);

            //if (this.IsEnabledMultiTenancy)
            //{
            //    var param = new DynamicData();
            //    param.Add(GeneratedField.TenantIdName, AppSession.TenantId);
            //    return SqlHelper.ExecuteScalar<long>(sql, param);
            //}
            //else
            //{
            //    return SqlHelper.ExecuteScalar<long>(sql);
            //}
        }

        internal long GetSerialNumber()
        {
            var query = CodeArt.DomainDriven.DataAccess.GetSerialNumber.Create(this);
            var sql = query.Build(null, this);

            if (this.IsSessionEnabledMultiTenancy)
            {
                var param = new DynamicData();
                param.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                return SqlHelper.ExecuteScalar<long>(sql, param);
            }
            else
            {
                return SqlHelper.ExecuteScalar<long>(sql);
            }
        }

        /// <summary>
        /// 获取表相对于<paramref name="parent"/>的属性路径下的对象链，格式是 a_b_c
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public string GetChainCode(DataTable parent)
        {
            return this.Chain.GetPathCode(parent);
        }

        /// <summary>
        /// 是否为<paramref name="target"/>的表或者为继承<paramref name="target"/>的表
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool IsEqualsOrDerivedOrInherited(DataTable target)
        {
            if (this.Name == target.Name) return true;
            return this.InheritedRoot.Name == target.InheritedRoot.Name;
        }


        /// <summary>
        /// 将DataFieldType.ValueList的值转换成可以存储的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static IEnumerable GetValueListData(object value)
        {
            if (value == null) Array.Empty<object>();

            var list = value as IEnumerable;
            var elementType = list.GetType().ResolveElementType();
            if(elementType.IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(elementType);
                return list.OfType<object>().Select((item) =>
                {
                    return DataUtil.ToValue(item, underlyingType);
                });
            }
            else
            {
                return list;
            }
        }

        private static Func<string, string> _getNameWithSeparated = LazyIndexer.Init<string, string>((name) =>
        {
            return string.Format("{0}_", name);
        });

        private static Func<string, string> _getNextName = LazyIndexer.Init<string, string>((name) =>
        {
            var pos = name.IndexOf("_");
            return name.Substring(pos + 1);
        });

        private static Func<string, string> _getIdName = LazyIndexer.Init<string, string>((name) =>
        {
            return string.Format("{0}{1}", name, EntityObject.IdPropertyName);
        });

        private static object GetObjectId(object obj)
        {
            var eo = obj as IEntityObject;
            if (eo != null) return eo.GetIdentity();

            var vo = obj as IValueObject;
            if (vo != null) return vo.Id; //生成的编号

            var dto = obj as DTObject; //测试时经常会用dto模拟对象
            if (dto != null) return dto.GetValue("Id");

            throw new DataAccessException(string.Format(Strings.UnableGetId, obj.GetType().ResolveName()));
        }

        /// <summary>
        /// 获取对象内部的原始数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static DynamicData GetOriginalData(object obj)
        {
            return ((obj as DomainObject).DataProxy as DataProxyPro).OriginalData;
        }

        #region 得到所有引用了该表的中间表信息

        ///// <summary>
        ///// 得到所有引用了目标表的中间表信息（目标表作为slave存在于中间表中）
        ///// </summary>
        ///// <param name="table"></param>
        ///// <param name="propertyName"></param>
        ///// <returns></returns>
        //private static IEnumerable<DataTable> GetQuoteMiddlesBySlave(DataTable table)
        //{
        //    return _getQuoteMiddlesBySlave(table);
        //}

        //private static Func<DataTable, IEnumerable<DataTable>> _getQuoteMiddlesBySlave
        //                                = LazyIndexer.Init<DataTable, IEnumerable<DataTable>>((target) =>
        //                                {
        //                                    var root = target.Root;
        //                                    List<DataTable> tables = new List<DataTable>();
        //                                    using (var temp = TempIndex.Borrow())
        //                                    {
        //                                        var index = temp.Item;
        //                                        FillQuoteMiddlesBySlave(target, root, tables, index);
        //                                    }
        //                                    return tables;
        //                                });

        //private static void FillQuoteMiddlesBySlave(DataTable target, DataTable current, List<DataTable> result, TempIndex index)
        //{
        //    foreach (var child in current.BuildtimeChilds)
        //    {
        //        if (!index.TryAdd(child)) continue; //尝试添加索引失败，证明已经处理，这通常是由于循环引用导致的死循环，用临时索引可以避免该问题

        //        if (child.Middle != null
        //            && child.Middle.Slave.Name.EqualsIgnoreCase(target.Name))
        //        {
        //            result.Add(child.Middle);
        //        }
        //        FillQuoteMiddlesBySlave(target, child, result, index);
        //    }
        //}


        /// <summary>
        /// 得到所有引用了目标表的中间表信息（目标表作为master存在于中间表中）
        /// </summary>
        /// <param name="table"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static IEnumerable<DataTable> GetQuoteMiddlesByMaster(DataTable table)
        {
            return _getQuoteMiddlesByMaster(table);
        }

        private static Func<DataTable, IEnumerable<DataTable>> _getQuoteMiddlesByMaster
                                        = LazyIndexer.Init<DataTable, IEnumerable<DataTable>>((target) =>
                                        {
                                            var root = target.Root;
                                            List<DataTable> tables = new List<DataTable>();
                                            using (var temp = TempIndex.Borrow())
                                            {
                                                var index = temp.Item;
                                                FillQuoteMiddlesByMaster(target, root, tables, index);
                                            }
                                            return tables;
                                        });

        private static void FillQuoteMiddlesByMaster(DataTable target, DataTable current, List<DataTable> result, TempIndex index)
        {
            foreach (var child in current.BuildtimeChilds)
            {
                if (!index.TryAdd(child)) continue; //尝试添加索引失败，证明已经处理，这通常是由于循环引用导致的死循环，用临时索引可以避免该问题
                if(child.Middle != null 
                    && child.Middle.Master.Name.EqualsIgnoreCase(target.Name))
                {
                    if(result.FirstOrDefault((t)=> { return t.Name == child.Middle.Name; })==null)
                        result.Add(child.Middle);
                }
                FillQuoteMiddlesByMaster(target, child, result, index);
            }
        }

        #endregion


        #region 根据属性名称，获取对应的对象引用的字段，也就是类型 属性名Id的形式的字段

        /// <summary>
        /// 根据属性名称，获取对应的对象引用的字段，也就是类型 属性名Id的形式的字段
        /// </summary>
        /// <param name="table"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static IDataField GetQuoteField(DataTable table, string propertyName)
        {
            return _getQuoteField(table)(propertyName);
        }

        private static Func<DataTable, Func<string, IDataField>> _getQuoteField
                                = LazyIndexer.Init<DataTable, Func<string, IDataField>>((table) =>
                                {
                                    return LazyIndexer.Init<string, IDataField>((propertyName) =>
                                    {
                                        foreach (var field in table.Fields)
                                        {
                                            if (field.ParentMemberField == null) continue;
                                            var current = field.ParentMemberField;
                                            if (current.Tip.PropertyName.EqualsIgnoreCase(propertyName))
                                            {
                                                return field;
                                            }
                                        }
                                        return null;
                                    });
                                });

        #endregion


        #region 根据typeKey找表

        private static Dictionary<string, DataTable> _typeTables = new Dictionary<string, DataTable>();

        private static void AddTypTable(string typeKey,DataTable table)
        {
            lock(_typeTables)
            {
                if (!_typeTables.ContainsKey(typeKey))
                {
                    var absoluteTable = table.GetAbsolute();
                    if (!_typeTables.ContainsKey(typeKey)) //防止 table.GetAbsolute方法操作了_typeTables，这里再次判断下重复
                    {
                        _typeTables.Add(typeKey, absoluteTable);
                    }
                }
                    
            }
        }

        /// <summary>
        /// 该方法可以找到动态类型对应的表
        /// </summary>
        /// <param name="typeKey"></param>
        /// <returns></returns>
        private static DataTable GetDataTable(string typeKey)
        {
            DataTable value = null;
            if (_typeTables.TryGetValue(typeKey, out value)) return value;
            throw new DomainDrivenException(string.Format(Strings.NotFoundDerivedType, typeKey));
        }

        #endregion

        #region 引用操作

        /// <summary>
        /// 递增引用次数
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        private void IncrementAssociated(object rootId, object id)
        {
            OperateAssociated(rootId, id, this.SqlIncrementAssociated);
        }

        private void OperateAssociated(object rootId, object id, string sql)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                data.Add(GeneratedField.RootIdName, rootId);
                data.Add(EntityObject.IdPropertyName, id);
                if (this.IsSessionEnabledMultiTenancy)
                    data.Add(GeneratedField.TenantIdName, AppSession.TenantId);
                SqlHelper.Execute(sql, data);
            }
        }

        /// <summary>
        /// 递减引用次数
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        private void DecrementAssociated(object rootId, object id)
        {
            OperateAssociated(rootId, id, this.SqlDecrementAssociated);
        }

        private int GetAssociated(object rootId, object id)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                data.Add(GeneratedField.RootIdName, rootId);
                data.Add(EntityObject.IdPropertyName, id);
                return SqlHelper.ExecuteScalar<int>(this.SqlSelectAssociated, data);
            }
        }



        private string _sqlIncrementAssociated = null;
        public string SqlIncrementAssociated
        {
            get
            {
                if (_sqlIncrementAssociated == null)
                {
                    _sqlIncrementAssociated = GetIncrementAssociatedSql();
                }
                return _sqlIncrementAssociated;
            }
        }

        private string GetIncrementAssociatedSql()
        {
            var query = DataAccess.IncrementAssociated.Create(this);
            return query.Build(null, this);
        }


        private string _sqlDecrementAssociated = null;
        public string SqlDecrementAssociated
        {
            get
            {
                if (_sqlDecrementAssociated == null)
                {
                    _sqlDecrementAssociated = GetDecrementAssociatedSql();
                }
                return _sqlDecrementAssociated;
            }
        }

        private string GetDecrementAssociatedSql()
        {
            var query = DataAccess.DecrementAssociated.Create(this);
            return query.Build(null, this);
        }


        private string _sqlSelectAssociated = null;
        public string SqlSelectAssociated
        {
            get
            {
                if (_sqlSelectAssociated == null)
                {
                    _sqlSelectAssociated = GetSelectAssociatedSql();
                }
                return _sqlSelectAssociated;
            }
        }

        private string GetSelectAssociatedSql()
        {
            var query = DataAccess.SelectAssociated.Create(this);
            return query.Build(null, this);
        }


        #endregion

        private void CheckDataVersion(DomainObject root)
        {
            var id = GetObjectId(root);
            if (root.DataVersion != this.GetDataVersion(id))
            {
                throw new DataVersionException(root.ObjectType, id);
            }
        }

    }
}