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
        public DataModel BaseModel
        {
            get;
            private set;
        }

        public Type ObjectType
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据模型对应的连接字符串的名称
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return this.Root.ConnectionName;
            }
        }

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

        private ObjectRepositoryAttribute _objectTip;


        private DataModel(Type objectType, DataTable root, DataTable snapshot)
        {
            this.ObjectType = objectType;
            this.Root = root;
            this.Snapshot = snapshot;
            _objectTip = this.Root.ObjectTip;
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
            if(_objectTip.Snapshot)
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

        public T QuerySingle<T>(object rootId, object id) where T : class, IDomainObject
        {
            return this.Root.QuerySingle(rootId, id) as T;
        }

        public T QuerySingle<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            return this.Root.QuerySingle<T>(expression, fillArg, level);
        }

        //public T QuerySingle<T>(IQueryBuilder query, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        //{
        //    return this.Root.QuerySingle<T>(query, fillArg, level);
        //}

        public IEnumerable<T> Query<T>(string expression, Action<DynamicData> fillArg, QueryLevel level) where T : class, IDomainObject
        {
            return this.Root.Query<T>(expression, fillArg, level);
        }

        //public IEnumerable<T> Query<T>(IQueryBuilder query, Action<DynamicData> fillArg) where T : class, IDomainObject
        //{
        //    return this.Root.Query<T>(query, fillArg);
        //}

        public Page<T> Query<T>(string expression, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IDomainObject
        {
            return this.Root.Query<T>(expression, pageIndex, pageSize, fillArg);
        }

        //public Page<T> Query<T>(IQueryBuilder pageQuery, IQueryBuilder countQuery, int pageIndex, int pageSize, Action<DynamicData> fillArg) where T : class, IDomainObject
        //{
        //    return this.Root.Query<T>(pageQuery, countQuery, pageIndex, pageSize, fillArg);
        //}

        public int GetCount(string expression, Action<DynamicData> fillArg, QueryLevel level)
        {
            return this.Root.GetCount(expression, fillArg, level);
        }

        //public int GetCount(IQueryBuilder query, Action<DynamicData> fillArg)
        //{
        //    return this.Root.GetCount(query, fillArg);
        //}

        /// <summary>
        /// 获取一个自增的编号
        /// </summary>
        /// <returns></returns>
        public long GetIdentity()
        {
            return this.Root.GetIdentity();
        }

        #region 辅助

        private static bool IsList(Type type)
        {
            return type.IsList();
        }

        #endregion

        #region 获得类型对应的数据字段

        internal static List<IDataField> GetObjectFields(Type objectType, bool isSnapshot)
        {
            return objectType.IsDerived() ? GetObjectFieldsByDerived(objectType, isSnapshot) : GetObjectFieldsByNoDerived(objectType, isSnapshot);
        }

        private static List<IDataField> GetObjectFieldsByDerived(Type objectType, bool isSnapshot)
        {
            var domainProperties = Util.GetProperties(objectType);
            var fields = GetFields(domainProperties, isSnapshot);
            return fields;
        }

        private static List<IDataField> GetObjectFieldsByNoDerived(Type objectType, bool isSnapshot)
        {
            var domainProperties = Util.GetProperties(objectType);
            var fields = GetFields(domainProperties, isSnapshot);

            fields.Add(GeneratedField.CreateTypeKey(objectType));
            fields.Add(GeneratedField.CreateDataVersion(objectType));
            return fields;
        }

        private static List<IDataField> GetFields(IEnumerable<DomainProperty> domainProperties, bool isSnapshot)
        {
            List<IDataField> fields = new List<IDataField>();
            foreach (var domainProperty in domainProperties)
            {
                var attr = domainProperty.RepositoryTip;
                if (attr == null) continue; //没有定义仓储特性，那么不持久化
                IDataField field = null;
                if (TryGetField(attr, isSnapshot, ref field))
                {
                    fields.Add(field);
                }
            }
            return fields;
        }


        private static bool TryGetField(PropertyRepositoryAttribute attribute, bool isSnapshot, ref IDataField result)
        {
            if (isSnapshot && !attribute.Snapshot) return false; //如果该模型是快照，但是属性定义没有加入快照，那么忽略该属性

            Type propertyType = attribute.PropertyType;
            switch (attribute.DomainPropertyType)
            {
                case DomainPropertyType.ValueObject:
                    {
                        var field = new ValueObjectField(attribute);
                        var childs = GetObjectFields(propertyType, isSnapshot);
                        field.AddChilds(childs);

                        result = field;
                        return true;
                    }
                case DomainPropertyType.AggregateRoot:
                    {
                        //引用了根对象
                        var field = new AggregateRootField(attribute);
                        result = field;
                        return true;
                    }
                case DomainPropertyType.EntityObject:
                    {
                        //引用了内部实体对象
                        var field = new EntityObjectField(attribute);
                        var childs = GetObjectFields(propertyType, isSnapshot);
                        field.AddChilds(childs);
                        result = field;
                        return true;
                    }
                case DomainPropertyType.EntityObjectPro:
                    {
                        var field = new EntityObjectProField(attribute);
                        var childs = GetObjectFields(propertyType, isSnapshot);
                        field.AddChilds(childs);
                        result = field;
                        return true;
                    }
                case DomainPropertyType.Primitive:
                    {
                        //普通的值数据
                        var field = attribute.PropertyIsId() ?
                                        new ValueField(attribute, DbFieldType.PrimaryKey) :
                                        new ValueField(attribute);
                        result = field;
                        return true;
                    }
            }

            if (IsList(propertyType))
            {
                IDataField field = null;
                if (TryGetListField(attribute, isSnapshot, ref field))
                {
                    result = field;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取集合类型的数据字段
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static bool TryGetListField(PropertyRepositoryAttribute attribute, bool isSnapshot, ref IDataField result)
        {
            var elementType = attribute.PropertyType.ResolveElementType();
            if (DomainObject.IsValueObject(elementType))
            {
                //值对象
                var field = new ValueObjectListField(attribute);
                var childs = GetObjectFields(elementType, isSnapshot);
                field.AddChilds(childs);

                result = field;
                return true;
            }
            else if (DomainObject.IsAggregateRoot(elementType))
            {
                //引用了根对象
                var field = new AggregateRootListField(attribute);
                result = field;
                return true;
            }
            else if (DomainObject.IsEntityObject(elementType))
            {
                //引用了内部实体对象
                var field = new EntityObjectListField(attribute);
                var childs = GetObjectFields(elementType, isSnapshot);
                field.AddChilds(childs);

                result = field;
                return true;
            }
            else if (DomainObject.IsEntityObjectPro(elementType))
            {
                var field = new EntityObjectProListField(attribute);
                var childs = GetObjectFields(elementType, isSnapshot);
                field.AddChilds(childs);

                result = field;
                return true;
            }
            else if (IsList(elementType))
            {
                throw new DomainDesignException(Strings.NestedCollection);
            }
            else
            {
                //值集合
                var field = new ValueListField(attribute);
                result = field;
                return true;
            }
        }

        #endregion

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
            var objectFields = GetObjectFields(objectType, false);
            var root = DataTable.Create(objectType, objectFields);

            var snapshotObjectFields = GetObjectFields(objectType, true);
            var snapshot = DataTable.CreateSnapshot(objectType, snapshotObjectFields);
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

        /// <summary>
        /// 找出当前应用程序可能涉及到的表信息
        /// </summary>
        private static IEnumerable<string> _indexs;

        static DataModel()
        {
            DomainObject.Initialize();
            _indexs = GetIndexs();
        }

        private static IEnumerable<string> GetIndexs()
        {
            DomainObject.CheckInitialized();

            List<string> tables = new List<string>();
            foreach (var objectType in DomainObject.TypeIndex)
            {
                if (DomainObject.IsEmpty(objectType)) continue;
                var fileds = GetObjectFields(objectType, false);
                tables.Add(objectType.Name);
                tables.AddRange(DataTable.GetRelatedNames(objectType, fileds));
            }
            return tables.Distinct();
        }

    }
}
