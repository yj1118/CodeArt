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
using System.Xml.Linq;

namespace CodeArt.DomainDriven.DataAccess
{
    public partial class DataTable
    {
        private List<DataTable> _buildtimeChilds;

        /// <summary>
        /// 构建时确定的表集合，该集合会运用查询
        /// 使用该属性时注意由于对象关系的循环引用导致的死循环问题
        /// </summary>
        public IEnumerable<DataTable> BuildtimeChilds
        {
            get
            {
                return _buildtimeChilds;
            }
        }

        //private List<DataTable> _runtimeChilds;

        //private void AddRuntimeChild(DataTable table)
        //{
        //    _runtimeChilds.Add(table);
        //}

        /// <summary>
        /// 包括预定义类型的表和运行时识别的类型的表
        /// </summary>
        //public IEnumerable<DataTable> RuntimeChilds
        //{
        //    get
        //    {
        //        return _runtimeChilds;
        //    }
        //}

        /// <summary>
        /// 初始子表
        /// </summary>
        private void InitChilds()
        {
            InitGetRuntimeTable();
            InitBuildtimeChilds();
        }

        #region 构建时的表

        /// <summary>
        /// 初始化构建时的表
        /// </summary>
        private void InitBuildtimeChilds()
        {
            _buildtimeChilds = new List<DataTable>();

            //初始化相关的基础表
            foreach (var field in this.ObjectFields)
            {
                var type = field.GetPropertyType();
                if (type.IsList())
                    type = field.Tip.GetElementType();
                if (field.FieldType == DataFieldType.GeneratedField
                    || field.FieldType == DataFieldType.Value) continue;

                var table = CreateChildTable(this, field, type);
                _buildtimeChilds.Add(table);
            }
        }

        #endregion


        private DataTable CreateChildTable(DataTable master, IDataField memberField, Type objectType)
        {
            DataTable root = null;
            if (this.Root == null || this.IsAggregateRoot) root = this; //就算有this.Root也要判断表是否为根对象的表，如果是为根对象的表，那么root就是自己
            else root = this.Root;

            DataTable table = null;
            switch (memberField.FieldType)
            {
                case DataFieldType.ValueObject:
                    {
                        table = CreateValueObjectTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.EntityObject:
                    {
                        table = CreateEntityObjectTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.EntityObjectList:
                    {
                        table = CreateEntityObjectListTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.ValueObjectList:
                    {
                        table = CreateValueObjectListTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.ValueList:
                    {
                        table = CreateValueListTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.AggregateRoot:
                    {
                        table = CreateAggregateRootTable(root, master, memberField, objectType);
                        break;
                    }
                case DataFieldType.AggregateRootList:
                    {
                        table = CreateAggregateRootListTable(root, master, memberField, objectType);
                        break;
                    }
            }

            if (table == null)
                throw new DataAccessException(string.Format(Strings.CreateChildTable, master.Name, memberField.FieldType, ObjectType.FullName));
            //master.AddRuntimeChild(table);
            return table;
        }

        /// <summary>
        /// 获得类型<paramref name="objectType"/> 可能涉及到的表的名称集合
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectFields"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetRelatedNames(Type objectType, IEnumerable<IDataField> objectFields)
        {
            List<string> names = new List<string>();
            foreach (var field in objectFields)
            {
                switch (field.FieldType)
                {
                    case DataFieldType.EntityObjectList:
                    case DataFieldType.ValueObjectList:
                    case DataFieldType.AggregateRootList:
                        {
                            names.Add(string.Format("{0}_{1}", objectType.Name, field.GetPropertyName()));
                        }
                        break;
                }
            }
            return names.Distinct();
        }

        /// <summary>
        /// 创建值对象的表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="master"></param>
        /// <param name="memberField"></param>
        /// <param name="isMultiple"></param>
        /// <returns></returns>
        private static DataTable CreateValueObjectTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            var fields = new List<IDataField>();
            fields.Add(GeneratedField.CreateValueObjectPrimaryKey(objectType));  //追加主键
            var mapper = DataMapperFactory.Create(objectType);
            fields.AddRange(mapper.GetObjectFields(objectType, master.IsSnapshot)); //增加对象定义的领域属性

            if (!objectType.IsDerived())
            {
                //派生类不必追加引用次数
                fields.Add(GeneratedField.CreateAssociatedCount(objectType));//追加被引用次数
            }

            return DataTable.Create(root,
                                    master,
                                    objectType,
                                    master.IsSnapshot,//根据主表，判断是否为快照
                                    DataTableType.ValueObject,
                                    fields,
                                    memberField);
        }



        /// <summary>
        /// 创建实体对象的表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="master"></param>
        /// <param name="memberField"></param>
        /// <param name="isMultiple"></param>
        /// <returns></returns>
        private static DataTable CreateEntityObjectTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            //注意，在内聚模型中，只要是实体对象，那么它就是相对于内聚根的实体对象，而不是所在对象的实体对象
            //因此，所有的实体对象，外键存放的都是内聚根的编号
            var fields = new List<IDataField>();
            var mapper = DataMapperFactory.Create(objectType);
            fields.AddRange(mapper.GetObjectFields(objectType, master.IsSnapshot)); //增加对象定义的领域属性
            if(!objectType.IsDerived())
            {
                //派生类不必有引用次数
                fields.Add(GeneratedField.CreateAssociatedCount(objectType));//追加被引用次数
            }

            return DataTable.Create(root,
                                master,
                                objectType,
                                master.IsSnapshot,//根据从表，判断是否为快照
                                DataTableType.EntityObject, fields, memberField);
        }

        /// <summary>
        /// 创建根内部引用外部根时映射的表
        /// </summary>
        /// <param name="master"></param>
        /// <param name="memberField"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static DataTable CreateAggregateRootTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            //var model = DataModel.CreateNew(objectType);
            //var table = model.Root;
            //table.Root = root;
            //table.Master = master;
            //table.MemberField = memberField;
            //return table;
            var mapper = DataMapperFactory.Create(objectType);
            var objectFields = mapper.GetObjectFields(objectType, master.IsSnapshot);
            var table = Create(root, master, objectType, master.IsSnapshot, DataTableType.AggregateRoot, objectFields, memberField);
            return table;
            //return DataTable.Create(root, master, objectType, DataTableType.AggregateRoot, memberField);
        }


        private static DataTable CreateValueObjectListTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            var slave = CreateValueObjectTable(root, master, memberField, objectType);
            var middle = CreateMiddleTable(slave, memberField);
            slave.Middle = middle;
            return slave;
        }

        /// <summary>
        /// 类似List（int）这样的值成员的集合所对应的表
        /// </summary>
        /// <param name="root"></param>
        /// <param name="master"></param>
        /// <param name="memberField"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        private static DataTable CreateValueListTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            var valueListField = memberField as ValueListField;
            string tableName = string.Format("{0}_{1}", master.Name, memberField.Name);
            IDataField[] fields = null;
            var reflectedType = valueListField.GetReflectedType(); //就算集合的成员类型各自不同，但是他们肯定继承至同一个根类，因此中间表是统一一个类型的

            if (root.IsEqualsOrDerivedOrInherited(master))
            {
                var rootField = GetForeignKey(master, GeneratedFieldType.RootKey, DbFieldType.NonclusteredIndex);
                var indexField = GeneratedField.CreateOrderIndex(objectType, DbFieldType.NonclusteredIndex);
                var valueField = GeneratedField.CreatePrimitiveValue(reflectedType, valueListField);
                fields = new IDataField[] { rootField, indexField, valueField };
            }
            else
            {
                var rootField = GetForeignKey(root, GeneratedFieldType.RootKey, DbFieldType.NonclusteredIndex);  //中间表中追加根字段，可以有效防止数据重叠
                var masterField = GetForeignKey(master, GeneratedFieldType.MasterKey, DbFieldType.NonclusteredIndex);
                var indexField = GeneratedField.CreateOrderIndex(objectType, DbFieldType.NonclusteredIndex);
                var valueField = GeneratedField.CreatePrimitiveValue(reflectedType, valueListField);

                fields = new IDataField[] { rootField, masterField, indexField, valueField };
            }

            var middle = DataTable.CreateMiddle(root,
                                    master,
                                    tableName,
                                    DataTableType.Middle,
                                    fields,
                                    memberField);
            return middle;
        }

        private static DataTable CreateEntityObjectListTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            //需要创建EntityObject从表和中间表
            var slave = CreateEntityObjectTable(root, master, memberField, objectType);
            var middle = CreateMiddleTable(slave, memberField);
            slave.Middle = middle;
            return slave;
        }

        private static DataTable CreateAggregateRootListTable(DataTable root, DataTable master, IDataField memberField, Type objectType)
        {
            //字段为根对象的集合，那么仅创建中间表
            var slave = DataTable.CreateAggregateRootTable(root, master, memberField, objectType);

            //var rootModal = DataModel.CreateNew(objectType);
            //var slave = rootModal.Root;
            //slave.SetInfo(root, master, memberField, master.IsSnapshot);
            var middle = CreateMiddleTable(slave, memberField);
            slave.Middle = middle;
            return slave;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="master"></param>
        /// <param name="slave"></param>
        /// <param name="memberField"></param>
        /// <returns></returns>
        private static DataTable CreateMiddleTable(DataTable slave, IDataField memberField)
        {
            var root = slave.Root;
            var master = slave.Master;
            
            string tableName = string.Format("{0}_{1}", master.Name, memberField.Name);
            IDataField[] fields = null;
            var objectType = (memberField as ObjectField).GetReflectedType(); //就算集合的成员类型各自不同，但是他们肯定继承至同一个根类，因此中间表是统一一个类型的

            if (root.IsEqualsOrDerivedOrInherited(master))
            {
                var rootField = GetForeignKey(master, GeneratedFieldType.RootKey, DbFieldType.NonclusteredIndex);
                var slaveField = GetForeignKey(slave, GeneratedFieldType.SlaveKey, DbFieldType.NonclusteredIndex);
                slaveField.ParentMemberField = memberField;

                var indexField = GeneratedField.CreateOrderIndex(objectType, DbFieldType.NonclusteredIndex);

                //注意，大多数查询都是以rootField 为条件, indexField为排序，输出slaveField字段，
                //所以slaveField的位置在最后
                fields = new IDataField[] { rootField, indexField, slaveField };
            }
            else
            {
                var rootField = GetForeignKey(root, GeneratedFieldType.RootKey, DbFieldType.NonclusteredIndex);  //中间表中追加根字段，可以有效防止数据重叠
                var masterField = GetForeignKey(master, GeneratedFieldType.MasterKey, DbFieldType.NonclusteredIndex);
                var slaveField = GetForeignKey(slave, GeneratedFieldType.SlaveKey, DbFieldType.NonclusteredIndex);
                slaveField.ParentMemberField = memberField;

                var indexField = GeneratedField.CreateOrderIndex(objectType, DbFieldType.NonclusteredIndex);

                //注意，大多数查询都是以rootField, masterField为条件, indexField为排序，输出slaveField字段，
                //所以slaveField的位置在最后
                fields = new IDataField[] { rootField, masterField, indexField, slaveField };
            }

            var middle = DataTable.CreateMiddle(root,
                                    master,
                                    tableName,
                                    DataTableType.Middle,
                                    fields,
                                    memberField);
            middle.Slave = slave;

            //如果从表是根，那么需要记录从表和中间表的联系，当删除根对象时，会删除该中间表的数据
            RootIsSlaveIndex.TryAdd(middle);

            return middle;
        }


        /// <summary>
        /// 获得以<paramref name="table"/>为主表的外键
        /// </summary>
        /// <param name="master"></param>
        /// <returns></returns>
        private static ValueField GetForeignKey(DataTable table, GeneratedFieldType keyType, params DbFieldType[] dbFieldTypes)
        {
            if (table.IdField == null)
                throw new InvalidOperationException("表" + table.Name + "没有id字段无法获得以它为主表的外键信息");
            string name = table.TableIdName;
            switch(keyType)
            {
                case GeneratedFieldType.RootKey:
                    {
                        name = GeneratedField.RootIdName;
                    }
                    break;
                case GeneratedFieldType.MasterKey:
                    {
                        name = GeneratedField.MasterIdName;
                    }
                    break;
                case GeneratedFieldType.SlaveKey:
                    {
                        name = GeneratedField.SlaveIdName;
                    }
                    break;
            }
            return new GeneratedField(table.IdField.Tip, name, keyType, dbFieldTypes);
        }

        #region 运行时

        /// <summary>
        /// 在运行时查找子表信息
        /// </summary>
        /// <param name="master"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private DataTable GetChildTableByRuntime(DataTable master, PropertyRepositoryAttribute tip)
        {
            return _getRuntimeTable(master)(tip.PropertyName)(tip.PropertyType);
        }



        /// <summary>
        /// 根据属性名称，实际类型，获取对应的表信息
        /// <para>runtime表包括预定义的类型和运行时加载的类型对应的表</para>
        /// <para>本来通过master和master下的属性名称PropertyName，我们就可以找到子表的定义</para>
        /// <para>但是由于属性的值有可能是不同的类型（因为继承关系），所以我们还需要指定属性的类型objectType</para>
        /// <para>才能获得当前属性对应的表信息</para>
        /// </summary>
        /// <param name="master"></param>
        /// <param name="propertyName"></param>
        /// <param name="objectType">表示实际运行时propertyName对应的objectType</param>
        /// <returns></returns>
        private DataTable GetRuntimeTable(DataTable master, string propertyName, Type objectType)
        {
            return _getRuntimeTable(master)(propertyName)(objectType);
        }
        
        private Func<DataTable, Func<string, Func<Type, DataTable>>> _getRuntimeTable;

        private void InitGetRuntimeTable()
        {
            _getRuntimeTable = LazyIndexer.Init<DataTable, Func<string, Func<Type, DataTable>>>((master) =>
                            {
                                return LazyIndexer.Init<string, Func<Type, DataTable>>((propertyName) =>
                                {
                                    return LazyIndexer.Init<Type, DataTable>((propertyType) =>
                                    {
                                        var memberField = master.ObjectFields.FirstOrDefault((field) =>
                                        {
                                            return field.GetPropertyName().EqualsIgnoreCase(propertyName);
                                        });

                                        if (memberField == null)
                                        {
                                            //如果派生类中找不到子表，那么在基类中继续找
                                            if(master.IsDerived)
                                            {
                                                return GetRuntimeTable(master.BaseTable, propertyName, propertyType);
                                            }

                                            throw new DataAccessException(string.Format(Strings.NotFoundTableField, master.Name, propertyName));
                                        }

                                        Type objectType = propertyType.IsList()
                                            ? memberField.Tip.GetElementType()
                                            : propertyType;

                                        return CreateChildTable(master, memberField, objectType);
                                    });
                                });
                            });
        }

        


        #endregion
    }
}