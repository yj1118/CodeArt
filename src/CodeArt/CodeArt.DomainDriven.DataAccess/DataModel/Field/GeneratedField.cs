using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Diagnostics;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.DomainDriven;


namespace CodeArt.DomainDriven.DataAccess
{
    /// <summary>
    /// 由orm生成的键
    /// </summary>
    internal sealed class GeneratedField : ValueField
    {
        public override DataFieldType FieldType
        {
            get
            {
                return DataFieldType.GeneratedField;
            }
        }

        public GeneratedFieldType GeneratedFieldType
        {
            get;
            private set;
        }

        public GeneratedField(PropertyRepositoryAttribute attribute, string name, GeneratedFieldType generatedFieldType, params DbFieldType[] dbFieldTypes)
            : base(attribute, dbFieldTypes)
        {
            this.Name = name;
            this.GeneratedFieldType = generatedFieldType;
        }


        public static GeneratedField CreateValueObjectPrimaryKey(Type reflectedType)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new GuidProperty(reflectedType, EntityObject.IdPropertyName)
            };
            return new GeneratedField(attr, EntityObject.IdPropertyName, GeneratedFieldType.ValueObjectPrimaryKey, DbFieldType.PrimaryKey);
        }

        /// <summary>
        /// 创建引用次数的键
        /// </summary>
        /// <param name="reflectedType"></param>
        /// <returns></returns>
        public static GeneratedField CreateAssociatedCount(Type reflectedType)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new IntProperty(reflectedType,AssociatedCountName)
            };
            return new GeneratedField(attr, AssociatedCountName, GeneratedFieldType.AssociatedCount, DbFieldType.Common);
        }

        /// <summary>
        /// 领域类型的编号字段
        /// </summary>
        /// <param name="reflectedType"></param>
        /// <returns></returns>
        public static GeneratedField CreateTypeKey(Type reflectedType)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new StringProperty(reflectedType, TypeKeyName, 50, true)
            };
            return new GeneratedField(attr, TypeKeyName, GeneratedFieldType.TypeKey, DbFieldType.Common);
        }

        /// <summary>
        /// 更新时间的字段
        /// </summary>
        /// <param name="reflectedType"></param>
        /// <returns></returns>
        public static GeneratedField CreateDataVersion(Type reflectedType)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new IntProperty(reflectedType, DataVersionName)
            };
            return new GeneratedField(attr, DataVersionName, GeneratedFieldType.DataVersion, DbFieldType.Common);
        }

        public const string AssociatedCountName = "AssociatedCount";
        public const string OrderIndexName = "OrderIndex";
        public const string DataVersionName = "DataVersion";
        public const string TypeKeyName = "TypeKey";

        public const string RootIdName = "RootId";
        public const string MasterIdName = "MasterId";
        public const string SlaveIdName = "SlaveId";
        public const string PrimitiveValueName = "Value";
        public const string TenantIdName = "TenantId";

        /// <summary>
        /// 创建中间表多个数据的排序序号键
        /// </summary>
        /// <param name="reflectedType"></param>
        /// <returns></returns>
        public static GeneratedField CreateOrderIndex(Type reflectedType, params DbFieldType[] types)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new IntProperty(reflectedType, OrderIndexName)
            };
            return new GeneratedField(attr, OrderIndexName, GeneratedFieldType.Index, types);
        }

        public static GeneratedField Create(Type ownerType, Type propertyType, string name)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new CustomProperty(ownerType, propertyType, name)
            };
            return new GeneratedField(attr, name, GeneratedFieldType.User);
        }

        /// <summary>
        /// 创建基础值集合的值字段
        /// </summary>
        /// <param name="ownerType"></param>
        /// <param name="propertyType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GeneratedField CreatePrimitiveValue(Type ownerType, ValueListField field)
        {
            var valueType = field.ValueType;
            DomainProperty property = null;
            DbFieldType fieldType = DbFieldType.Common;
            if (valueType == typeof(string))
            {
                var maxLength = field.Tip.GetMaxLength();
                if(maxLength < 300)
                {
                    //如果value的字符串类型长度小于300，那么就可以参与索引
                    fieldType = DbFieldType.NonclusteredIndex;
                }
                property = new StringProperty(ownerType, PrimitiveValueName, maxLength, field.Tip.IsASCIIString());
            }
            else
            {
                fieldType = DbFieldType.NonclusteredIndex;
                property = new CustomProperty(ownerType, valueType, PrimitiveValueName);
            }

            var attr = new PropertyRepositoryAttribute()
            {
                Property = property
            };
            return new GeneratedField(attr, PrimitiveValueName, GeneratedFieldType.PrimitiveValue, fieldType);
        }



        public static GeneratedField CreateString(Type ownerType, string name, int maxLength, bool ascii)
        {
            var attr = new PropertyRepositoryAttribute()
            {
                Property = new StringProperty(ownerType, name, maxLength, ascii)
            };
            return new GeneratedField(attr, name, GeneratedFieldType.User);
        }



        internal class CustomProperty : DomainProperty
        {
            public CustomProperty(Type ownerType, Type propertyType, string name)
            {
                this.OwnerType = ownerType;
                this.PropertyType = propertyType;
                this.Name = name;
            }
        }


        private class GuidProperty : CustomProperty
        {
            public GuidProperty(Type ownerType, string name)
                : base(ownerType, typeof(Guid), name)
            {
            }
        }

        private class IntProperty : CustomProperty
        {
            public IntProperty(Type ownerType, string name)
                : base(ownerType, typeof(int), name)
            {
            }
        }

        internal class StringProperty : CustomProperty
        {
            public int MaxLength
            {
                get;
                private set;
            }

            public bool IsASCII
            {
                get;
                private set;
            }


            public StringProperty(Type ownerType, string name,int maxLength,bool isASCII)
                : base(ownerType, typeof(string), name)
            {
                this.MaxLength = maxLength;
                this.IsASCII = isASCII;
            }
        }
    }

    internal enum GeneratedFieldType
    {
        RootKey,
        MasterKey,
        SlaveKey,
        /// <summary>
        /// 值对象的主键
        /// </summary>
        ValueObjectPrimaryKey,
        /// <summary>
        /// 值集合的值字段
        /// </summary>
        PrimitiveValue,
        /// <summary>
        /// 标示数据被引用了多少次的键
        /// </summary>
        AssociatedCount,
        /// <summary>
        /// 中间表中用来存序号的键
        /// </summary>
        Index,
        /// <summary>
        /// 领域类型的编号
        /// </summary>
        TypeKey,
        /// <summary>
        /// 数据版本号
        /// </summary>
        DataVersion,
        User //表示用户自定义的字段
    }


}
