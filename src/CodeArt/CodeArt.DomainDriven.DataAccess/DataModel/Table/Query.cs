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
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        /// <summary>
        /// 根据编号查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal object QuerySingle(object id, QueryLevel level)
        {
            var obj = GetObjectFromConstruct(id);
            if (obj != null) return obj;

            var expression = GetObjectByIdExpression(this);
            return QuerySingle(expression, (param) =>
            {
                param.Add(EntityObject.IdPropertyName, id);
            }, level);
        }

        /// <summary>
        /// 1对1引用关系的读取
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private object QuerySingle(object rootId, object id)
        {
            var obj = GetObjectFromConstruct(rootId, id);
            if (obj != null) return obj;

            var expression = GetObjectByIdExpression(this);
            return QuerySingle(expression, (param) =>
            {
                param.Add(GeneratedField.RootIdName, rootId);
                param.Add(EntityObject.IdPropertyName, id);
            }, QueryLevel.None);
        }

        #region 从构造上下文中获取对象

        /// <summary>
        /// 从构造上下文中获取对象
        /// </summary>
        /// <returns></returns>
        private object GetObjectFromConstruct(object id)
        {
            return ConstructContext.Get(this.ObjectType, id);
        }

        /// <summary>
        /// 从构造上下文中获取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private object GetObjectFromConstruct(object rootId, object id)
        {
            return ConstructContext.Get(this.ObjectType, rootId, id);
        }

        private object GetObjectFromConstruct(DynamicData data)
        {
            var id = data.Get(EntityObject.IdPropertyName);
            if (this.Type == DataTableType.AggregateRoot)
            {
                return GetObjectFromConstruct(id);
            }
            var rootId = data.Get(GeneratedField.RootIdName);
            return GetObjectFromConstruct(rootId, id);
        }

        #endregion

        /// <summary>
        /// 根据表达式查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="fillArg"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        internal T QuerySingle<T>(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            return (T)QuerySingle(expression, fillArg, level);
        }

        internal object QuerySingle(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            expression = GetEntryExpression(expression);
            var exp = QueryObject.Create(this, expression, level);

            object result = null;
            UseDataEntry(exp, fillArg, (entry) =>
            {
                result = GetObjectFromEntry(entry, level);
            });
            return result != null ? result : DomainObject.GetEmpty(this.ObjectType);
        }

        /// <summary>
        /// 该方法用于快速加载对象
        /// </summary>
        /// <returns></returns>
        private object GetObjectFromEntryByPage(DynamicData entry)
        {
            string typeKey = (string)entry.Get(GeneratedField.TypeKeyName);
            var table = string.IsNullOrEmpty(typeKey) ? this : GetDataTable(typeKey);

            //非聚合根是不能被加入到缓冲区的
            return table.CreateObject(table.ObjectTip.ObjectType, entry, QueryLevel.None);
        }

        /// <summary>
        /// 从条目数据中获取对象
        /// <para>如果加载的对象是聚合根，那么如果存在缓冲区中，那么使用缓冲区的对象，如果不存在，则在数据库中查询并更新缓冲区</para>
        /// <para>如果加载的是成员对象，那么始终从数据库中查询</para>
        /// <para>不论是什么类型的对象都可以识别继承类型</para>
        /// </summary>
        /// <returns></returns>
        private object GetObjectFromEntry(DynamicData entry, QueryLevel level)
        {
            if (this.Type == DataTableType.AggregateRoot)
            {
                object id = entry.Get(EntityObject.IdPropertyName);
                int dataVersion = (int)entry.Get(GeneratedField.DataVersionName);
                string typeKey = (string)entry.Get(GeneratedField.TypeKeyName);
                var table = string.IsNullOrEmpty(typeKey) ? this : GetDataTable(typeKey);

                if (level.Code == QueryLevel.MirroringCode)
                {
                    //镜像查询会在加入到当前会话中的对象缓冲区，不同的会话有各自的镜像对象，同一个会话的同一个对象的镜像只有一个
                    var obj = DomainBuffer.Mirror.GetOrCreate(table.ObjectTip.ObjectType, id, dataVersion, () =>
                    {
                        return (IAggregateRoot)table.LoadObject(id, QueryLevel.Mirroring); //由于查询条目的时候已经锁了数据，所以此处不用再锁定
                    });
                    DataContext.Current.AddMirror(obj); //加入镜像
                    return obj;
                }


                {
                    var obj = DomainBuffer.Public.GetOrCreate(table.ObjectTip.ObjectType, id, dataVersion, () =>
                    {
                        return (IAggregateRoot)table.LoadObject(id, QueryLevel.None); //由于查询条目的时候已经锁了数据，所以此处不用再锁定
                    });
                    if(DataContext.ExistCurrent())
                        DataContext.Current.AddBuffer(obj); //加入数据上下文缓冲区
                    return obj;
                }
            }
            else
            {
                object rootId = entry.Get(GeneratedField.RootIdName);
                object id = entry.Get(EntityObject.IdPropertyName);
                int dataVersion = (int)entry.Get(GeneratedField.DataVersionName);
                string typeKey = (string)entry.Get(GeneratedField.TypeKeyName);
                var table = string.IsNullOrEmpty(typeKey) ? this : GetDataTable(typeKey);

                //非聚合根是不能被加入到缓冲区的
                return table.LoadObject(rootId, id);
            }
        }

        internal IEnumerable<T> Query<T>(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            expression = GetEntryExpression(expression);
            var exp = QueryObject.Create(this, expression, level);
            return GetObjectsFromEntries<T>(exp, fillArg, level);
        }

        internal Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg)
        {
            expression = GetEntryExpression(expression);
            var exp = QueryPage.Create(this, expression);
            IEnumerable<T> objects = GetObjectsFromEntriesByPage<T>(exp, (data) =>
            {
                data.Add("pageIndex", pageIndex);
                data.Add("pageSize", pageSize);
                fillArg(data);
            });
            int count = GetCount(expression, fillArg, QueryLevel.None);
            return new Page<T>(pageIndex, pageSize, objects, count);
        }

        internal int GetCount(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            //获取总数据数
            var exp = QueryCount.Create(this, expression, level);
            return GetCount(exp, fillArg);
        }

        private int GetCount(IQueryBuilder query, Action<DynamicData> fillArg)
        {
            int count = 0;
            //获取总数据数
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                fillArg(param);
                AddToTenant(param);
                var sql = query.Build(param, this);
                count = SqlHelper.ExecuteScalar<int>(sql, param);
            }
            return count;
        }

        /// <summary>
        /// 执行一个持久层命令
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fillArg"></param>
        /// <param name="level"></param>
        internal void Execute(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            //获取总数据数
            var exp = QueryCommand.Create(this, expression, level);
            Execute(exp, fillArg);
        }

        private void Execute(IQueryBuilder query, Action<DynamicData> fillArg)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                fillArg(param);
                AddToTenant(param);

                var sql = query.Build(param, this);
                SqlHelper.Execute(sql, param);
            }
        }

        private IEnumerable<T> GetObjectsFromEntriesByPage<T>(IQueryBuilder exp, Action<DynamicData> fillArg)
        {
            var list = Symbiosis.TryMark(ListPool<T>.Instance, () =>
            {
                return new List<T>();
            });

            UseDataEntries(exp, fillArg, (entry) =>
            {
                var obj = GetObjectFromEntryByPage(entry);
                list.Add((T)obj);
            });
            return list;
        }

        private IEnumerable<T> GetObjectsFromEntries<T>(IQueryBuilder exp, Action<DynamicData> fillArg, QueryLevel level)
        {
            var list = Symbiosis.TryMark(ListPool<T>.Instance, () =>
            {
                return new List<T>();
            });

            UseDataEntries(exp, fillArg, (entry) =>
            {
                var obj = GetObjectFromEntry(entry, level);
                list.Add((T)obj);
            });
            return list;
        }


        #region 获取条目信息


        /// <summary>
        /// 获取数据的条目信息，这包括数据必备的编号和版本号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="dataVersion"></param>
        /// <returns></returns>
        private void UseDataEntry(IQueryBuilder exp, Action<DynamicData> fillArg, Action<DynamicData> action)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var data = temp.Item;
                ExecuteQueryEntry(exp, fillArg, data);
                if (data.Count > 0)
                {
                    action(data);
                }
            }
        }

        /// <summary>
        /// 使用条目集合
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fillArg"></param>
        /// <param name="level"></param>
        /// <param name="action"></param>
        private void UseDataEntries(IQueryBuilder exp, Action<DynamicData> fillArg, Action<DynamicData> action)
        {
            using (var temp = SqlHelper.BorrowDatas())
            {
                var datas = temp.Item;
                ExecuteQueryEntries(exp, fillArg, datas);
                foreach (var data in datas)
                    action(data);
            }
        }


        private void ExecuteQueryEntry(IQueryBuilder query, Action<DynamicData> fillArg, DynamicData data)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                fillArg(param);
                AddToTenant(param);

                var sql = query.Build(param, this);//编译表达式获取执行文本
                SqlHelper.QueryFirstOrDefault(sql, param, data);
            }
        }

        private void AddToTenant(DynamicData param)
        {
            //if (this.IsEnabledMultiTenancy) 不一定是判断this表，因为在关联表查询中，有可能是Middle表是开启了租户，但是被引用的根表（例如User）没有开启租户，这时候还是要给租户条件
            // 为了方便编码，我们认为只要配置文件启动了多租户，在传递条件时，我们就会赋予租户的参数
            if(DomainDrivenConfiguration.Current.MultiTenancyConfig.IsEnabled)
            {
                if(!param.ContainsKey(GeneratedField.TenantIdName))  //TenantId有可能被业务使用,被业务指定了查询条件
                    param.Add(GeneratedField.TenantIdName, AppSession.TenantId);
            }
        }


        private void ExecuteQueryEntries(IQueryBuilder query, Action<DynamicData> fillArg, List<DynamicData> datas)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                fillArg(param);
                AddToTenant(param);

                var sql = query.Build(param, this);//编译表达式获取执行文本
                SqlHelper.Query(sql, param, datas);
            }
        }

        /// <summary>
        /// 根据表达式获取数据条目
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetEntryExpression(string expression)
        {
            return _getFindEntryByExpression(this)(expression)(this.IsSessionEnabledMultiTenancy);
        }

        /// <summary>
        /// 获得默认查询的字段
        /// </summary>
        /// <returns></returns>
        private static string GetDefaultQueryFields(DataTable table)
        {
            StringBuilder fields= new StringBuilder();

            foreach (var field in table.DefaultQueryFields)
            {
                fields.AppendFormat("{0},", field.Name);
            }

            fields.Length--;

            return fields.ToString();
        }


        private static Func<DataTable, Func<string, Func<bool, string>>> _getFindEntryByExpression = LazyIndexer.Init<DataTable, Func<string, Func<bool, string>>>((table) =>
        {
            return LazyIndexer.Init<string, Func<bool, string>>((expression) =>
            {
                return LazyIndexer.Init<bool, string>((isEnabledMultiTenancy) =>
                {
                    SqlDefinition def = SqlDefinition.Create(expression, isEnabledMultiTenancy);
                    if (def.IsCustom) return expression; //如果是自定义查询，那么直接返回表达式，由程序员自行解析
                    var selects = def.Columns.Select;
                    if (selects.Count() == 0 || selects.First() == "*")
                    {
                        var tenantSql = isEnabledMultiTenancy  ? string.Format(",{0}", GeneratedField.TenantIdName) : string.Empty;

                        var fieldsSql = GetDefaultQueryFields(table);

                        expression = string.Format("{0}[select {1}{2}]", expression, fieldsSql, tenantSql);

                        //没有指定输出属性就输出对象自身
                        //if (table.Type == DataTableType.AggregateRoot)
                        //{
                        //    expression = string.Format("{0}[select {1},{2},{3}{4}]", expression, EntityObject.IdPropertyName, GeneratedField.DataVersionName, GeneratedField.TypeKeyName, tenantSql);
                        //}
                        //else
                        //{
                        //    expression = string.Format("{0}[select {1},{2},{3},{4}{5}]", expression, GeneratedField.RootIdName, EntityObject.IdPropertyName, GeneratedField.DataVersionName, GeneratedField.TypeKeyName, tenantSql);
                        //}
                    }
                    else if (selects.Count() == 1)
                    {
                        var propertyName = selects.First();

                        var tenantSql = isEnabledMultiTenancy ? string.Format(",{0}.{1} as {1}", propertyName, GeneratedField.TenantIdName) : string.Empty;

                        if (table.Type == DataTableType.AggregateRoot)
                        {
                            expression = string.Format("{0}[select {1}.{2} as {2},{1}.{3} as {3},{1}.{4} as {4}{5}]", expression, propertyName, EntityObject.IdPropertyName, GeneratedField.DataVersionName, GeneratedField.TypeKeyName, tenantSql);
                        }
                        else
                        {
                            expression = string.Format("{0}[select {1},{2}.{3},{2}.{4},{2}.{5}{6}]", expression, GeneratedField.RootIdName, propertyName, EntityObject.IdPropertyName, GeneratedField.DataVersionName, GeneratedField.TypeKeyName, tenantSql);
                        }
                    }
                    else if (selects.Count() > 1)
                    {
                        throw new DataAccessException(string.Format(Strings.NotSupportMultipleProperties, expression));
                    }

                    return expression;
                });
            });
        });


        #endregion

        #region 实际查询，构造完整对象

        /// <summary>
        /// 根据编号从数据库中查询数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private DomainObject LoadObject(object id, QueryLevel level)
        {
            var expression = GetObjectByIdExpression(this);
            var exp = QueryObject.Create(this, expression, level);
            return ExecuteQueryObject(this.ObjectType, exp, (param) =>
            {
                param.Add(EntityObject.IdPropertyName, id);
            }, level);
        }

        /// <summary>
        /// 根据编号从数据库中查询数据
        /// </summary>
        /// <param name="rootId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private DomainObject LoadObject(object rootId, object id)
        {
            var expression = GetObjectByIdExpression(this);
            var exp = QueryObject.Create(this, expression, QueryLevel.None);
            return ExecuteQueryObject(this.ObjectType, exp, (param) =>
            {
                param.Add(GeneratedField.RootIdName, rootId);
                param.Add(EntityObject.IdPropertyName, id);
            }, QueryLevel.None);
        }

        ///// <summary>
        ///// 根据表达式从数据库中查询数据
        ///// </summary>
        ///// <param name="expression"></param>
        ///// <param name="fillArg"></param>
        ///// <param name="level"></param>
        ///// <returns></returns>
        //private object LoadObject(string expression, Action<DynamicData> fillArg, QueryLevel level)
        //{
        //    var exp = QueryObject.Create(this, expression, level);
        //    return ExecuteQueryObject(this.ImplementOrElementType, exp, fillArg);
        //}

        private DomainObject ExecuteQueryObject(Type objectType, IQueryBuilder query, Action<DynamicData> fillArg, QueryLevel level)
        {
            using (var temp = SqlHelper.BorrowData())
            {
                var param = temp.Item;
                fillArg(param);
                AddToTenant(param);

                var sql = query.Build(param, this);//编译表达式获取执行文本
                var data = SqlHelper.QueryFirstOrDefault(sql, param);
                return CreateObject(objectType, data, level);
            }
        }

        /// <summary>
        /// 执行查询对象集合的sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="fillArg"></param>
        /// <returns></returns>
        //private IEnumerable<T> ExecuteQueryObjects<T>(IQueryBuilder query, Action<DynamicData> fillArg)
        //{
        //    using (var paraTemp = SqlHelper.BorrowData())
        //    {
        //        var param = paraTemp.Item;
        //        fillArg(param);
        //        var sql = query.Build(param);

        //        using (var temp = SqlHelper.BorrowDatas())
        //        {
        //            var datas = temp.Item;
        //            SqlHelper.Query(this.ConnectionName, sql, param, datas);

        //            var tempList = ListPool<T>.Borrow();
        //            Symbiosis.Current.Mark(tempList);
        //            var list = tempList.Item;

        //            var elementType = typeof(T);
        //            foreach (var data in datas)
        //            {
        //                var item = (T)CreateObject(elementType, data);
        //                list.Add(item);
        //            }
        //            return list;
        //        }
        //    }
        //}

        #endregion

        #region 得到对象版本号


        /// <summary>
        /// 根据对象原始数据获取版本号
        /// </summary>
        internal int GetDataVersion(DynamicData originalData)
        {
            if(this.Type == DataTableType.AggregateRoot)
            {
                var id = originalData.Get(EntityObject.IdPropertyName);
                return GetDataVersion(id);
            }
            else
            {
                var rootId = originalData.Get(GeneratedField.RootIdName);
                var id = originalData.Get(EntityObject.IdPropertyName);
                return GetDataVersion(rootId, id);
            }
        }


        /// <summary>
        /// 得到对象的版本号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private int GetDataVersion(object id)
        {
            return GetDataVersionImpl((param) =>
            {
                param.Add(EntityObject.IdPropertyName, id);
            });
        }

        private int GetDataVersion(object rootId, object id)
        {
            return GetDataVersionImpl((param) =>
            {
                param.Add(GeneratedField.RootIdName, rootId);
                param.Add(EntityObject.IdPropertyName, id);
            });
        }

        private int GetDataVersionImpl(Action<DynamicData> fillArg)
        {
            var expression = GetObjectByIdExpression(this);
            var exp = QueryObject.Create(this, expression, QueryLevel.None); //在领域层主动查看数据编号时，不需要锁；在提交数据时获取数据编号，由于对象已被锁，所以不需要在读取版本号时锁

            int dataVersion = 0;
            UseDataEntry(exp, fillArg, (entry) =>
            {
                dataVersion = (int)entry.Get(GeneratedField.DataVersionName);
            });
            return dataVersion;
        }

        #endregion


        //internal T QuerySingle<T>(IQueryBuilder compiler, Action<DynamicData> fillArg, QueryLevel level)
        //{
        //    return (T)ExecuteQueryObject(this.ImplementOrElementType, compiler, fillArg);
        //}



        //internal IEnumerable<T> Query<T>(IQueryBuilder query, Action<DynamicData> fillArg)
        //{
        //    return ExecuteQueryObjects<T>(query, fillArg);
        //}



        //internal Page<T> Query<T>(IQueryBuilder pageQuery, IQueryBuilder countQuery, int pageIndex, int pageSize, Action<DynamicData> fillArg)
        //{
        //    IList<T> objects = ExecuteQueryObjects<T>(pageQuery, (data) =>
        //    {
        //        data.Add("pageIndex", pageIndex);
        //        data.Add("pageSize", pageSize);
        //        fillArg(data);
        //    });
        //    int count = GetCount(countQuery, fillArg);
        //    return new Page<T>(pageIndex, pageSize, objects, count);
        //}



        private static string GetObjectByIdExpression(DataTable table)
        {
            return _getObjectIdExpression(table)(table.IsSessionEnabledMultiTenancy);
        }

        private static Func<DataTable, Func<bool,string>> _getObjectIdExpression = LazyIndexer.Init<DataTable, Func<bool, string>>((table) =>
        {
            return LazyIndexer.Init<bool, string>((isEnabledMultiTenancy) =>
            {
                string expression = null;
                if (table.Type == DataTableType.AggregateRoot)
                {
                    expression = string.Format("{0}=@{0}", EntityObject.IdPropertyName);
                }
                else
                {
                    expression = string.Format("{0}=@{0} and {1}=@{1}", GeneratedField.RootIdName, EntityObject.IdPropertyName);
                }

                if (isEnabledMultiTenancy)
                {
                    expression = string.Format("{0} and {1}=@{1}", expression, GeneratedField.TenantIdName);
                }

                return expression;
            });

            
        });
    }
}