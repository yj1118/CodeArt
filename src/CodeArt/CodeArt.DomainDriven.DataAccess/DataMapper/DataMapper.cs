using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Runtime;
using CodeArt.Concurrent;

namespace CodeArt.DomainDriven.DataAccess
{
    [SafeAccess]
    public class DataMapper : IDataMapper
    {
        public DataMapper()
        {

        }

        //public virtual void FillInsertData(DomainObject obj, DynamicData data, DataTable table) { }

        public virtual void OnPreInsert(DomainObject obj, DataTable table) { }


        public virtual void OnInserted(DomainObject obj, DataTable table) { }

        //public virtual void FillUpdateData(DomainObject obj, DynamicData data, DataTable table) { }

        public virtual void OnPreUpdate(DomainObject obj, DataTable table) { }


        public virtual void OnUpdated(DomainObject obj, DataTable table) { }

        public virtual void OnPreDelete(DomainObject obj, DataTable table) { }


        public virtual void OnDeleted(DomainObject obj, DataTable table) { }

        #region 获得类型对应的数据字段


        public IEnumerable<IDataField> GetObjectFields(Type objectType, bool isSnapshot)
        {
            var fields = GetObjectFieldsByGenerate(objectType, isSnapshot);
            //附加字段
            var attachedFields = GetAttachFields(objectType, isSnapshot);
            fields.AddRange(MapFields(objectType, attachedFields));

            return fields;
        }

        private IEnumerable<IDataField> MapFields(Type objectType, IEnumerable<DbField> attachedFields)
        {
            List<IDataField> fields = new List<IDataField>();
            foreach (var attachedField in attachedFields)
            {
                var stringField = attachedField as StringField;
                if (stringField != null)
                {
                    var field = GeneratedField.CreateString(objectType, stringField.Name, stringField.MaxLength, stringField.ASCII);
                    field.IsAdditional = true;
                    fields.Add(field);
                }
                else
                {
                    var field = GeneratedField.Create(objectType, attachedField.ValueType, attachedField.Name);
                    field.IsAdditional = true;
                    fields.Add(field);
                }
            }
            return fields;
        }


        protected virtual IEnumerable<DbField> GetAttachFields(Type objectType, bool isSnapshot)
        {
            return Array.Empty<DbField>();
        }


        private List<IDataField> GetObjectFieldsByGenerate(Type objectType, bool isSnapshot)
        {
            return objectType.IsDerived() ? GetObjectFieldsByDerived(objectType, isSnapshot) : GetObjectFieldsByNoDerived(objectType, isSnapshot);
        }


        private List<IDataField> GetObjectFieldsByDerived(Type objectType, bool isSnapshot)
        {
            var domainProperties = Util.GetProperties(objectType);
            var fields = GetFields(domainProperties, isSnapshot);
            return fields;
        }

        private List<IDataField> GetObjectFieldsByNoDerived(Type objectType, bool isSnapshot)
        {
            var domainProperties = Util.GetProperties(objectType);
            var fields = GetFields(domainProperties, isSnapshot);

            fields.Add(GeneratedField.CreateTypeKey(objectType));
            fields.Add(GeneratedField.CreateDataVersion(objectType));
            return fields;
        }

        private List<IDataField> GetFields(IEnumerable<DomainProperty> domainProperties, bool isSnapshot)
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


        private bool TryGetField(PropertyRepositoryAttribute attribute, bool isSnapshot, ref IDataField result)
        {
            if (isSnapshot && !attribute.Snapshot) return false; //如果该模型是快照，但是属性定义没有加入快照，那么忽略该属性

            Type propertyType = attribute.PropertyType;
            switch (attribute.DomainPropertyType)
            {
                case DomainPropertyType.ValueObject:
                    {
                        var field = new ValueObjectField(attribute);
                        //var mapper = DataMapperFactory.Create(propertyType);
                        //var childs = mapper.GetObjectFields(propertyType, isSnapshot);
                        //field.AddChilds(childs);

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
                        //var mapper = DataMapperFactory.Create(propertyType);
                        //var childs = mapper.GetObjectFields(propertyType, isSnapshot);
                        //field.AddChilds(childs);
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

            if (propertyType.IsList())
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
        private bool TryGetListField(PropertyRepositoryAttribute attribute, bool isSnapshot, ref IDataField result)
        {
            var elementType = attribute.GetElementType();
            if (DomainObject.IsValueObject(elementType))
            {
                //值对象
                var field = new ValueObjectListField(attribute);
                //当值对象A引用了值对象A时，会造成死循环
                //var mapper = DataMapperFactory.Create(elementType);
                //var childs = mapper.GetObjectFields(elementType, isSnapshot);
                //field.AddChilds(childs);

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
                //当成员对象A引用了成员对象A时，会造成死循环
                //var mapper = DataMapperFactory.Create(elementType);
                //var childs = mapper.GetObjectFields(elementType, isSnapshot);
                //field.AddChilds(childs);

                result = field;
                return true;
            }
            else if (elementType.IsList())
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

        public virtual string Build(QueryBuilder builder, DynamicData param, DataTable table)
        {
            return string.Empty;
        }

        #region 静态成员

        public static readonly DataMapper Instance = new DataMapper();

        /// <summary>
        /// 关键字段的定义，只要通过关键字段的查询，ORM引擎就可以加载完整的根对象，该属性用于自定义查询的select语句中
        /// </summary>
        public static readonly string KeyFields = null;

        static DataMapper()
        {
            KeyFields = string.Format("{0},{1},{2}", EntityObject.IdPropertyName, GeneratedField.DataVersionName, GeneratedField.TypeKeyName);
        }

        #endregion

    }
}
