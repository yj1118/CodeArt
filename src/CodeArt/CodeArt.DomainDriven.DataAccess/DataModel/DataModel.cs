using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;
using CodeArt.DTO;

namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 规则:
    /// 1.EntityObjectPro的存储，由外界显示调用，ORM不会在存储内聚根的时候自动保存和自动删除
    /// 2.EntityObjectPro被其他属性引用，这种引用关系，ORM会自动存储
    /// 3.EntityObject的存储，在存储内聚根的时候自动保存和自动删除（如果没有引用了，就删除）
    /// </summary>
    public sealed class DataModel
    {
        public Type ObjectType
        {
            get;
            private set;
        }

        ///// <summary>
        ///// 数据模型对应的连接字符串的名称
        ///// </summary>
        //public string ConnectionName
        //{
        //    get
        //    {
        //        return this.Root.ConnectionName;
        //    }
        //}

        /// <summary>
        /// 根表的信息
        /// </summary>
        internal DataTable Root
        {
            get;
            private set;
        }

        /// <summary>
        /// 快照表
        /// </summary>
        internal DataTable Snapshot
        {
            get;
            private set;
        }

        public ObjectRepositoryAttribute ObjectTip
        {
            get;
            private set;
        }


        private DataModel(Type objectType, DataTable root, DataTable snapshot)
        {
            this.ObjectType = objectType;
            this.Root = root;
            this.Snapshot = snapshot;
            this.ObjectTip = this.Root.ObjectTip;
        }

        public void Insert(DomainObject obj)
        {
            this.Root.Insert(obj);
        }

        public void Update(DomainObject obj)
        {
            this.Root.Update(obj);
        }

        public void Delete(DomainObject obj)
        {
            if(ObjectTip.Snapshot)
            {
                //开启快照功能，先保存快照
                this.Snapshot.Insert(obj);
            }
            this.Root.Delete(obj);
        }

        public T QuerySingle<T>(object id, QueryLevel level) where T : class, IDomainObject
        {
            return this.Root.QuerySingle(id, level) as T;
        }

        public T QuerySingle<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            return this.Root.QuerySingle<T>(expression, fillArg, level);
        }

        public IEnumerable<T> Query<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            return this.Root.Query<T>(expression, fillArg, level);
        }


        public Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IDomainObject
        {
            return this.Root.Query<T>(expression, pageIndex, pageSize, fillArg);
        }


        public int GetCount(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            return this.Root.GetCount(expression, fillArg, level);
        }

        public void Execute(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            this.Root.Execute(expression, fillArg, level);
        }

        /// <summary>
        /// 获取一个自增的编号
        /// </summary>
        /// <returns></returns>
        public long GetIdentity()
        {
            return this.Root.GetIdentity();
        }

        public long GetSerialNumber()
        {
            return this.Root.GetSerialNumber();
        }

        #region 静态成员

        private static Func<Type, DataModel> _getDataModel = LazyIndexer.Init<Type, DataModel>((objectType) =>
        {
            return CreateNew(objectType);
        });


        internal static DataModel Create(Type objectType)
        {
            return _getDataModel(objectType);
        }

        internal static DataModel CreateNew(Type objectType)
        {
            var mapper = DataMapperFactory.Create(objectType);

            var objectFields = mapper.GetObjectFields(objectType, false);
            var root = DataTable.Create(objectType, objectFields);

            DataTable snapshot = null;
            if (DomainObject.IsAggregateRoot(objectType))
            {
                var tip = ObjectRepositoryAttribute.GetTip(objectType, true);
                if (tip.Snapshot)
                {
                    var snapshotObjectFields = mapper.GetObjectFields(objectType, true);
                    snapshot = DataTable.CreateSnapshot(objectType, snapshotObjectFields);
                }
            }
            return new DataModel(objectType, root, snapshot);
        }


        #endregion


        ///// <summary>
        ///// 初始化数据模型，这在关系数据库中体现为创建表
        ///// <para>该方法适合单元测试的时候重复使用</para>
        ///// </summary>
        public static void RuntimeBuild()
        {
            DataTable.RuntimeBuild();
        }

        public static void Drop()
        {
            DataTable.Drop();
        }

        public static void ClearUp()
        {
            DataTable.ClearUp();
        }

        /// <summary>
        /// 找出当前应用程序可能涉及到的表信息
        /// </summary>
        private static IEnumerable<string> _indexs;

        static DataModel()
        {
            try
            {
                DomainObject.Initialize();
                _indexs = GetIndexs();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private static IEnumerable<string> GetIndexs()
        {
            DomainObject.CheckInitialized();

            List<string> tables = new List<string>();
            foreach (var objectType in DomainObject.TypeIndex)
            {
                if (DomainObject.IsEmpty(objectType)) continue;
                var mapper = DataMapperFactory.Create(objectType);
                var fileds = mapper.GetObjectFields(objectType, false);
                tables.Add(objectType.Name);
                tables.AddRange(DataTable.GetRelatedNames(objectType, fileds));
            }
            return tables.Distinct();
        }

    }
}
