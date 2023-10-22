using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.AppSetting;

namespace CodeArt.DomainDriven.DataAccess
{
    [DebuggerDisplay("Name = {Name}, UniqueKey = {UniqueKey}")]
    [SafeAccess]
    public partial class DataTable
    {
        /// <summary>
        /// 继承链中的父表
        /// </summary>
        public DataTable BaseTable
        {
            get;
            private set;
        }

        /// <summary>
        /// 继承链中的根类
        /// </summary>
        public DataTable InheritedRoot
        {
            get;
            private set;
        }

        /// <summary>
        /// 指示表是派生的
        /// </summary>
        public bool IsDerived
        {
            get
            {
                return this.DerivedClass != null;
            }
        }

        public DerivedClassAttribute DerivedClass
        {
            get;
            private set;
        }

        /// <summary>
        /// 表的继承链（不包括当前表）
        /// </summary>
        public IEnumerable<DataTable> Inheriteds
        {
            get;
            private set;
        }

        /// <summary>
        /// 包括当前表在内的派生链（不包括BaseRoot）
        /// </summary>
        public IEnumerable<DataTable> Deriveds
        {
            get;
            private set;
        }

        /// <summary>
        /// 对象的实际类型，对于中间表，该类型是集合类型
        /// </summary>
        public Type ObjectType
        {
            get;
            private set;
        }

        /// <summary>
        /// 该值仅在中间表时才有用
        /// </summary>
        public Type ElementType
        {
            get;
            private set;
        }


        public ObjectRepositoryAttribute ObjectTip
        {
            get;
            private set;
        }

        /// <summary>
        /// 该表是否为根表
        /// </summary>
        public bool IsAggregateRoot
        {
            get
            {
                //return this.ObjectTip == null ? false : DomainObject.IsAggregateRoot(this.ObjectTip.ObjectType);
                return this.ObjectType == null ? false : DomainObject.IsAggregateRoot(this.ObjectType); //有可能对象标记的ObjectTip的类型与对象的实际类型不同，所以需要使用ObjectType
            }
        }

        public bool IsSnapshot
        {
            get;
            private set;
        }

        public DomainProperty MemberDomainProperty
        {
            get
            {
                return this.MemberField?.Tip?.Property;
            }
        }

        public PropertyRepositoryAttribute MemberPropertyTip
        {
            get
            {
                return this.MemberField?.Tip;
            }
        }


        /// <summary>
        /// 格式为TableId的编号，例如：Book表的TableIdName就为 BookId
        /// </summary>
        public string TableIdName
        {
            get;
            private set;
        }

        /// <summary>
        /// 所属根表（领域根）
        /// </summary>
        public DataTable Root
        {
            get;
            private set;
        }

        /// <summary>
        /// <para>在引用链中的根表，这个属性大多数情况等属性Root，但是在跨根引用中会有所不同</para>
        /// <para>例如：根 role 引用了根 organization,而根organization又有属性permissions引用了多个根permission</para>
        /// <para>这时候对于permission表，Root是organization,ChainRoot是role</para>
        /// </summary>
        public DataTable ChainRoot
        {
            get;
            private set;
        }


        //public string ConnectionName
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// 所属主表
        /// </summary>
        public DataTable Master
        {
            get;
            private set;
        }

        /// <summary>
        /// 与该表相关的中间表
        /// </summary>
        public DataTable Middle
        {
            get;
            private set;
        }

        /// <summary>
        /// 该表相关的从表，该属性在middle表中存在
        /// </summary>
        public DataTable Slave
        {
            get;
            private set;
        }


        public DataTableType Type
        {
            get;
            private set;
        }

        /// <summary>
        ///  每次插入数据，是多条的
        /// </summary>
        public bool IsMultiple
        {
            get;
            private set;
        }

        /// <summary>
        /// 表示表是不是因为多条值数据（例如：List<int>）而创建的中间表
        /// </summary>
        public bool IsPrimitiveValue
        {
            get
            {
                return this.IsMultiple && this.Slave == null; //作为多条值数据的表，没有slave子表
            }
        }


        /// <summary>
        /// 表名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public bool IsSessionEnabledMultiTenancy
        {
            get
            {
                if(this.ObjectTip == null)
                {
                    //中间表是没有ObjectTip的
                    return this.Root.IsSessionEnabledMultiTenancy;
                }
                return IsEnabledMultiTenancy && AppSession.TenantEnabled;
            }
        }

        public bool IsEnabledMultiTenancy
        {
            get
            {
                if (this.ObjectTip == null)
                {
                    //中间表是没有ObjectTip的
                    return this.Root.IsEnabledMultiTenancy;
                }
                if (this.IsDynamic) return false; //动态类型是远程对象，远程对象不加租户编号
                return !this.ObjectTip.CloseMultiTenancy && DomainDrivenConfiguration.Current.MultiTenancyConfig.IsEnabled;
            }
        }

        public IEnumerable<IDataField> Fields
        {
            get;
            private set;
        }


        /// <summary>
        /// 默认查询的字段
        /// </summary>
        public IEnumerable<IDataField> DefaultQueryFields
        {
            get
            {
                return this.Fields.Where(field => !field.IsAdditional && !field.Tip.Lazy);
            }
        }

        public IEnumerable<IDataField> ObjectFields
        {
            get;
            private set;
        }

        private IDataField _slaveField;

        public IDataField SlaveField
        {
            get
            {
                if(this.Type == DataTableType.Middle)
                {
                    if (_slaveField == null)
                    {
                        _slaveField = this.Fields.First(t => (t as GeneratedField)?.GeneratedFieldType == GeneratedFieldType.SlaveKey);
                    }
                    return _slaveField;
                }
                return null;
            }
        }

        private IDataField _rootField;

        public IDataField RootField
        {
            get
            {
                if (this.Type == DataTableType.Middle)
                {
                    if (_rootField == null)
                    {
                        _rootField = this.Fields.First(t => (t as GeneratedField)?.GeneratedFieldType == GeneratedFieldType.RootKey);
                    }
                    return _rootField;
                }
                return null;
            }
        }


        private IDataField _idField;
        public IDataField IdField
        {
            get
            {
                if (_idField == null) _idField = this.Fields.FirstOrDefault((t) => t.Name == EntityObject.IdPropertyName);
                return _idField;
            }
        }

        private IEnumerable<IDataField> _primaryKeys;
        public IEnumerable<IDataField> PrimaryKeys
        {
            get
            {
                if (_primaryKeys == null) _primaryKeys = this.Fields.Where((t) => t.IsPrimaryKey);
                return _primaryKeys;
            }
        }

        private IEnumerable<IDataField> _clusteredIndexs;
        public IEnumerable<IDataField> ClusteredIndexs
        {
            get
            {
                if (_clusteredIndexs == null) _clusteredIndexs = this.Fields.Where((t) => t.IsClusteredIndex);
                return _clusteredIndexs;
            }
        }

        private IEnumerable<IDataField> _nonclusteredIndexs;
        public IEnumerable<IDataField> NonclusteredIndexs
        {
            get
            {
                if (_nonclusteredIndexs == null) _nonclusteredIndexs = this.Fields.Where((t) => t.IsNonclusteredIndex);
                return _nonclusteredIndexs;
            }
        }

        /// <summary>
        /// 该表在对象中所在成员的字段定义
        /// </summary>
        public IDataField MemberField
        {
            get;
            private set;
        }

        public IEnumerable<PropertyRepositoryAttribute> PropertyTips
        {
            get;
            private set;
        }

        /// <summary>
        /// 表的唯一键，在不同的对象关系下，同一个表会出现在不同的对象关系链中，该键可以体现这项特征
        /// </summary>
        public string UniqueKey
        {
            get;
            private set;
        }

        internal ObjectChain Chain
        {
            get;
            private set;
        }

        public string ChainCode
        {
            get
            {
                return this.Chain.PathCode;
            }
        }

        public IDataMapper Mapper
        {
            get;
            private set;
        }


        private DataTable(Type objectType, 
                            bool isSnapshot, 
                            DataTable chainRoot, 
                            DataTable master, 
                            DataTableType type, 
                            string name,
                            IEnumerable<IDataField> tableFields,
                            IEnumerable<IDataField> objectFields,
                            IDataField memberField)
        {
            this.UniqueKey = GetUniqueKey(memberField, chainRoot?.Name, name);
            AddBuildtimeIndex(this);
            if(memberField != null) memberField.Table = this;


            this.Type = type;

            this.ChainRoot = chainRoot;
            this.Master = master;
            this.MemberField = memberField;
            this.Root = FindActualRoot(chainRoot);
            InitObjectType(objectType, memberField?.Tip);

            this.Chain = this.MemberField == null ? ObjectChain.Empty : new ObjectChain(this.MemberField);
            this.IsSnapshot = isSnapshot;

            this.IsMultiple = memberField == null ? false : memberField.IsMultiple;

            this.Name = name;
            this.Fields = TidyFields(tableFields);

            this.ObjectFields = objectFields;
            this.PropertyTips = GetPropertyTips();
            InitDerived();
            //InitConnectionName();
            InitTableIdName();
            InitChilds();
            InitDynamic();
            this.Mapper = DataMapperFactory.Create(this.ObjectType);
            //这里触发，是为了防止程序员在程序启动时手工初始化，但会遗漏动态表的初始化
            //所以在表构造的时候主动创建
            this.Build();
        }

        private DataTable FindActualRoot(DataTable root)
        {
            if (this.Type == DataTableType.AggregateRoot && this.MemberField == null) return this;
            var master = this.Master;
            while(master != null)
            {
                if (master.Type == DataTableType.AggregateRoot) return master;
                master = master.Master;
            }
            return null;
        }

        private void InitDerived()
        {
            var objectType = this.ObjectType;
            var table = this;
            if (objectType.IsDerived())
            {
                DerivedClassAttribute.CheckUp(objectType);
                table.DerivedClass = DerivedClassAttribute.GetAttribute(objectType);
                AddTypTable(table.DerivedClass.TypeKey, table);
                table.Inheriteds = GetInheriteds();
                table.BaseTable = table.Inheriteds.Last();
                table.InheritedRoot = table.Inheriteds.First();
                var deriveds = table.Inheriteds.ToList();
                deriveds.RemoveAt(0); //删除baseRoot
                deriveds.Add(table);  //加上当前表
                table.Deriveds = deriveds;
            }
            else
            {
                table.InheritedRoot = table; //如果不是派生类，那么继承根就是自己
                table.Inheriteds = Array.Empty<DataTable>();
                table.Deriveds = Array.Empty<DataTable>();
            }
        }

        private IEnumerable<DataTable> GetInheriteds()
        {
            Stack<DataTable> inheriteds = new Stack<DataTable>();
            var baseType = this.ObjectType.BaseType;
            while (baseType != null && !DomainObject.IsMergeDomainType(baseType))
            {
                DataTable table = CreateInheritedTable(baseType);
                inheriteds.Push(table);
                baseType = baseType.BaseType;
            }
            return inheriteds;
        }

        private DataTable CreateInheritedTable(Type baseType)
        {
            switch (this.Type)
            {
                case DataTableType.AggregateRoot:
                    {
                        return this.IsSnapshot ? DataModel.Create(baseType).Snapshot
                                               : DataModel.Create(baseType).Root;
                    }
                case DataTableType.EntityObject:
                    {
                        return this.Root.CreateChildTable(this.Master, this.MemberField, baseType);

                        //return CreateEntityObjectTable(this.Root, this.Master, this.MemberField, baseType);
                    }
                case DataTableType.ValueObject:
                    {
                        return this.Root.CreateChildTable(this.Master, this.MemberField, baseType);
                        //return CreateValueObjectTable(this.Root, this.Master, this.MemberField, baseType);
                    }
            }
            throw new DataAccessException(string.Format(Strings.CreateInheritedTableError, baseType.FullName, this.Type.ToString()));
        }

        /// <summary>
        ///  我们要保证rootId在第一列，Id在第二列
        ///  这样不仅符合人们的操作习惯，在建立索引时，也会以rootId作为第一位，提高查询性能
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private IEnumerable<IDataField> TidyFields(IEnumerable<IDataField> fields)
        {
            if (this.Type == DataTableType.AggregateRoot || this.Type == DataTableType.Middle) return fields;

            if(this.Type == DataTableType.ValueObject || this.Type == DataTableType.EntityObject)
            {
                //需要补充根键，因为我们只有在内部得到了实际的root后才能算出根键
                var rootKey = GetForeignKey(this.Root, GeneratedFieldType.RootKey, DbFieldType.PrimaryKey);//根编号要放在最前面，方便优化索引性能
                var temp = new List<IDataField>();
                temp.Add(rootKey);
                temp.AddRange(fields);
                return temp;
            }

            return fields;
        }


        //private void InitConnectionName()
        //{
        //    if (this.IsMultiple)
        //    {
        //        this.ConnectionName = this.Root.ConnectionName;
        //    }
        //    else
        //    {
        //        //自身为根表，就取表名为连接字符串的名称
        //        this.ConnectionName = this.Type == DataTableType.AggregateRoot ? this.Name :this.Root.Name;
        //    }
        //}

        private void InitObjectType(Type objectType,PropertyRepositoryAttribute tip)
        {
            this.ObjectType = objectType;
            if (this.Type == DataTableType.Middle)
            {
                this.ElementType = tip.GetElementType();
            }
            else if(this.Type == DataTableType.AggregateRoot)
            {
                this.ObjectTip = ObjectRepositoryAttribute.GetTip(this.ObjectType, true);
            }
            else
            {
                //对于值对象和引用对象，如果没有定义ObjectRepositoryAttribute，那么使用根的ObjectRepositoryAttribute
                this.ObjectTip = ObjectRepositoryAttribute.GetTip(this.ObjectType, false);
                if(this.ObjectTip == null)
                {
                    this.ObjectTip = this.Root.ObjectTip;
                }
            }
        }

        private void InitTableIdName()
        {
            if (this.Type != DataTableType.Middle)
            {
                var type = this.ObjectType;
                //不用this.Name而用type.Name是因为有可能是快照表，表名称带快照前缀
                if(this.IsDerived)
                {
                    //派生类的TableIdName统一用基类的TableIdName
                    //这样可以避免基类和派生类都引用了同样的对象而导致外键编号名称不一样的麻烦
                    this.TableIdName = this.InheritedRoot.TableIdName;
                }
                else
                {
                    this.TableIdName = string.Format("{0}{1}", type.Name, EntityObject.IdPropertyName);
                }
            }
        }

        internal RuntimeObjectType DynamicType
        {
            get;
            private set;
        }
        
        public bool IsDynamic
        {
            get;
            private set;
        }

        private void InitDynamic()
        {
            this.DynamicType = this.ObjectType as RuntimeObjectType;
            this.IsDynamic = this.DynamicType != null;
            if (this.IsDynamic)
            {
                AddTypTable(this.DynamicType.Define.TypeName, this);
            }
        }


        private IEnumerable<PropertyRepositoryAttribute> GetPropertyTips()
        {
            return this.Fields.Select(t => t.Tip).Where(t => t != null).Distinct();
        }

        internal static DataTable Create(Type objectType, IEnumerable<IDataField> objectFields)
        {
            DataTableType tableType = DataTableType.AggregateRoot;
           
            if (DomainObject.IsAggregateRoot(objectType))
            {
                tableType = DataTableType.AggregateRoot;
                return Create(null, null, objectType, false, tableType, objectFields, null);
            }
            throw new DomainDrivenException(string.Format(Strings.PersistentObjectError, objectType.FullName));
        }


        //internal static DataTable Create(DataTable root, DataTable master, Type objectType, DataTableType tableType, IDataField memberField)
        //{
        //    var objectFields = DataModel.GetObjectFields(objectType, master.IsSnapshot);
        //    return Create(root, master, objectType, master.IsSnapshot, tableType, objectFields, memberField);
        //}


        internal static DataTable CreateSnapshot(Type objectType, IEnumerable<IDataField> objectFields)
        {
            if (!DomainObject.IsAggregateRoot(objectType)) throw new SnapshotTargetException();
            DataTableType tableType = DataTableType.AggregateRoot;
            return Create(null, null, objectType, true, tableType, objectFields, null);
        }


        private static string GetTableName(Type objectType, bool isSnapshot)
        {
            return isSnapshot ? string.Format("{0}_{1}", Snapshot, objectType.Name) : objectType.Name;
        }

        private static DataTable Create(DataTable root, DataTable master,
                                        Type objectType, bool isSnapshot, DataTableType tableType,
                                        IEnumerable<IDataField> objectFields, IDataField memberField)
        {
            var tableName = GetTableName(objectType, isSnapshot);
            return Create(root, master, objectType, isSnapshot, tableType, tableName, objectFields, memberField);
        }

        /// <summary>
        /// 创建中间表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="master"></param>
        /// <param name="tableName"></param>
        /// <param name="tableType"></param>
        /// <param name="isMultiple"></param>
        /// <param name="objectFields"></param>
        /// <param name="memberField"></param>
        /// <returns></returns>
        private static DataTable CreateMiddle(DataTable root, DataTable master,
                                string tableName,
                                DataTableType tableType,
                                IEnumerable<IDataField> objectFields, IDataField memberField)
        {
            // memberField.GetPropertyType()就是集合类型，中间表的objectType是集合类型
            return Create(root, master, memberField.GetPropertyType(), master.IsSnapshot, tableType, tableName, objectFields, memberField);
        }


        private static DataTable Create(DataTable root, DataTable master,
                                        Type objectType, bool isSnapshot, DataTableType tableType,
                                        string tableName, IEnumerable<IDataField> objectFields, IDataField memberField)
        {
            //补全memberField信息
            if(memberField != null)
            {
                if (memberField.ParentMemberField == null)
                    memberField.ParentMemberField = master?.MemberField;

                if (memberField.MasterTableName == null)
                    memberField.MasterTableName = master?.Name;

                if (memberField.TableName == null)
                    memberField.TableName = tableName;

            }

            var table = GetBuildtimeIndex(memberField, root?.Name, tableName);
            if (table != null) return table; //防止死循环
            var copyFields = objectFields.ToList();
            //获取字段信息
            var tableFields = GetTableFields(copyFields); //得到表需要存储的字段集合
            //得到表和子表信息
            table = new DataTable(objectType,
                                        isSnapshot,
                                        root,
                                        master,
                                        tableType,
                                        tableName,
                                        tableFields,
                                        objectFields,
                                        memberField);
            return table;
        }

        #region 过滤字段，得到表的字段信息

        protected static IEnumerable<IDataField> GetTableFields(List<IDataField> objectFields)
        {
            List<IDataField> fields = new List<IDataField>();
            for (var i = 0; i < objectFields.Count; i++)
            {
                var objectField = objectFields[i];
                FillFields(fields, objectField);
            }
            return fields;
        }

        private static bool FillFields(List<IDataField> fields, IDataField current)
        {
            string name = string.IsNullOrEmpty(current.Name) ? current.GetPropertyName() : current.Name;
            switch (current.FieldType)
            {
                case DataFieldType.GeneratedField:
                    {
                        fields.Add(current); //对于生成的键，直接追加
                        return true;
                    }
                case DataFieldType.Value:
                    {
                        var valueField = current as ValueField;
                        //存值
                        var field = new ValueField(current.Tip, valueField.DbFieldTypes.ToArray())
                        {
                            Name = name,
                            ParentMemberField = current.ParentMemberField
                        };
                        fields.Add(field);
                        return true;
                    }
                //case DataFieldType.ValueList:
                //    {
                //        //普通值的集合，会被转换成逗号分隔的字符串存放，因此作为字段输入
                //        var vlf = current as ValueListField;
                //        var field = new ValueListField(vlf.Tip)
                //        {
                //            Name = name
                //        };
                //        fields.Add(field);
                //        return true;
                //    }
                case DataFieldType.EntityObject:
                case DataFieldType.EntityObjectPro:
                case DataFieldType.AggregateRoot:
                    {
                        //存外键即可
                        var idAttr = DomainProperty.GetProperty(current.Tip.PropertyType, EntityObject.IdPropertyName).RepositoryTip;

                        var field = new ValueField(idAttr)
                        {
                            Name = _getIdName(name),
                            ParentMemberField = current
                        };
                        fields.Add(field);
                        return true;
                    }
                case DataFieldType.ValueObject:
                    {
                        var primaryKey = GeneratedField.CreateValueObjectPrimaryKey(current.Tip.PropertyType);
                        var field = new ValueField(primaryKey.Tip)
                        {
                            Name = _getIdName(name),
                            ParentMemberField = current
                        };
                        fields.Add(field);
                        return true;
                    }
                default:
                    {
                        current.Name = name; //对于其他的类型，只是赋值字段名称
                        break;
                    }
            }
            return false;
        }

        #endregion
    }

    public enum DataTableType
    {
        ValueObject,
        AggregateRoot,
        EntityObject,
        Middle //中间表
    }

}
