using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Data;

using CodeArt.Util;


namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 查询适配器，使用适配器可以兼容多个数据库的查询操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryAdapter<T>
        where T : class, IAggregateRoot
    {
        private Dictionary<string, string> _expressionData;

        private Dictionary<string, Action<DynamicData>> _fillArgsData;

        private bool _dataIsComplete;


        /// <summary>
        /// 适配器的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        private string _dbType;

        private QueryAdapter(string name)
        {
            this.Name = name;
            _dbType = SqlContext.GetDbType();
            _dataIsComplete = false;
        }

        private QueryAdapter<T> FillData(string dbType, string expression, Action<DynamicData> fillArgs)
        {
            ArgumentAssert.IsNotNull(expression, "expression");
            if (_expressionData == null) _expressionData = new Dictionary<string, string>(5);
            _expressionData.Add(dbType, expression);

            if (fillArgs != null)
            {
                if (_fillArgsData == null) _fillArgsData = new Dictionary<string, Action<DynamicData>>(5);
                _fillArgsData.Add(dbType, fillArgs);
            }
            return this;
        }

        public QueryAdapter<T> SQLServer(string expression, Action<DynamicData> fillArgs)
        {
            if (_dataIsComplete) return this;
            return FillData(DatabaseType.SQLServer, expression, fillArgs);
        }

        public QueryAdapter<T> SQLServer(string expression)
        {
            return SQLServer(expression, null);
        }

        public QueryAdapter<T> MySQL(string expression, Action<DynamicData> fillArgs)
        {
            if (_dataIsComplete) return this;
            return FillData(DatabaseType.MySQL, expression, fillArgs);
        }

        public QueryAdapter<T> MySQL(string expression)
        {
            return MySQL(expression, null);
        }

        public QueryAdapter<T> Oracle(string expression, Action<DynamicData> fillArgs)
        {
            if (_dataIsComplete) return this;
            return FillData(DatabaseType.Oracle, expression, fillArgs);
        }

        public QueryAdapter<T> Oracle(string expression)
        {
            return Oracle(expression, null);
        }

        public QueryAdapter<T> Access(string expression, Action<DynamicData> fillArgs)
        {
            if (_dataIsComplete) return this;
            return FillData(DatabaseType.Access, expression, fillArgs);
        }

        public QueryAdapter<T> Access(string expression)
        {
            return Access(expression, null);
        }

        #region 单条查询

        public T QuerySingle(Action<DynamicData> fillArg, QueryLevel level)
        {
            T obj = null;
            UsingExpression((express) =>
            {
                obj = DataContext.Current.QuerySingle<T>(express, fillArg, level);
            });
            return obj;
        }

        /// <summary>
        /// 仅指定查询级，表达式和填充参数都由各自的数据库定义决定
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public T QuerySingle(QueryLevel level)
        {
            T obj = null;
            UsingData((express, fillArg) =>
            {
                obj = DataContext.Current.QuerySingle<T>(express, fillArg, level);
            });
            return obj;
        }

        #endregion

        #region 集合查询

        public IEnumerable<T> Query(Action<DynamicData> fillArg, QueryLevel level)
        {
            IEnumerable<T> objs = null;
            UsingExpression((express) =>
            {
                objs = DataContext.Current.Query<T>(express, fillArg, level);
            });
            return objs;
        }

        /// <summary>
        /// 仅指定查询级，表达式和填充参数都由各自的数据库定义决定
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<T> Query(QueryLevel level)
        {
            IEnumerable<T> objs = null;
            UsingData((express, fillArg) =>
            {
                objs = DataContext.Current.Query<T>(express, fillArg, level);
            });
            return objs;
        }

        #endregion

        #region 分页查询

        public Page<T> Query(int pageIndex, int pageSize, Action<DynamicData> fillArg)
        {
            SetComplete();
            Page<T> page = default(Page<T>);
            UsingExpression((express) =>
            {
                page = DataContext.Current.Query<T>(express, pageIndex, pageSize, fillArg);
            });
            return page;
        }

        /// <summary>
        /// 仅指定页号和页大小，表达式和填充参数都由各自的数据库定义决定
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Page<T> Query(int pageIndex, int pageSize)
        {
            Page<T> page = default(Page<T>);
            UsingData((express, fillArg) =>
            {
                page = DataContext.Current.Query<T>(express, pageIndex, pageSize, fillArg);
            });
            return page;
        }


        #endregion

        #region 数据统计

        public int GetCount(Action<DynamicData> fillArg, QueryLevel level)
        {
            int count = 0;
            UsingExpression((express) =>
            {
                count = DataContext.Current.GetCount<T>(express, fillArg, level);
            });
            return count;
        }

        public int GetCount(QueryLevel level)
        {
            int count = 0;
            UsingData((express, fillArg) =>
            {
                count = DataContext.Current.GetCount<T>(express, fillArg, level);
            });
            return count;
        }

        #endregion

        #region 执行命令

        public void Execute(Action<DynamicData> fillArg, QueryLevel level)
        {
            UsingExpression((express) =>
            {
                DataContext.Current.Execute<T>(express, fillArg, level);
            });
        }

        /// <summary>
        /// 仅指定查询级，表达式和填充参数都由各自的数据库定义决定
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public void Execute(QueryLevel level)
        {
            UsingData((express, fillArg) =>
            {
                DataContext.Current.Execute<T>(express, fillArg, level);
            });
        }

        #endregion

        #region 重构

        private void SetComplete()
        {
            _dataIsComplete = true; //指示数据已经填充完毕,后续再执行时不会再填充数据
        }

        /// <summary>
        /// 使用定义的表达式
        /// </summary>
        /// <param name="action"></param>
        private void UsingExpression(Action<string> action)
        {
            SetComplete();
            string express = null;
            if (_expressionData.TryGetValue(_dbType, out express))
            {
                action(express);
                return;
            }
            throw new DataPortalException(string.Format(Strings.NotFoundQueryExpression, _dbType, typeof(T).FullName));
        }

        /// <summary>
        /// 使用定义的表达式和填充参数的方法
        /// </summary>
        /// <param name="action"></param>
        private void UsingData(Action<string, Action<DynamicData>> action)
        {
            SetComplete();
            string express = null;
            if (_expressionData.TryGetValue(_dbType, out express))
            {
                Action<DynamicData> fillArg = null;
                if (_fillArgsData.TryGetValue(_dbType, out fillArg))
                {
                    action(express, fillArg);
                    return;
                }
                throw new DataPortalException(string.Format(DataAccess.Strings.NotFoundFillArguments, typeof(T).FullName, this.Name, _dbType));
            }
            throw new DataPortalException(string.Format(DataAccess.Strings.NotFoundQueryExpression, typeof(T).FullName, this.Name, _dbType));
        }

        #endregion

        public static QueryAdapter<T> Create(string adapterName)
        {
            ArgumentAssert.IsNotNull(adapterName, "adapterName");
            return _getAdapter(adapterName);
        }

        private static Func<string, QueryAdapter<T>> _getAdapter = LazyIndexer.Init<string, QueryAdapter<T>>((adapterName) =>
        {
            return new QueryAdapter<T>(adapterName);
        });
    }
}
