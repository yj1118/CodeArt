using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CodeArt.DomainDriven;
using CodeArt.Runtime;
using CodeArt.Util;

namespace CodeArt.DomainDriven.DataAccess
{
    public static class Util
    {
        private static Dictionary<Type, DbType> _typeMap = new Dictionary<Type, DbType>
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(Emptyable<byte>)] = DbType.Byte,
            [typeof(Emptyable<sbyte>)] = DbType.SByte,
            [typeof(Emptyable<short>)] = DbType.Int16,
            [typeof(Emptyable<ushort>)] = DbType.UInt16,
            [typeof(Emptyable<int>)] = DbType.Int32,
            [typeof(Emptyable<uint>)] = DbType.UInt32,
            [typeof(Emptyable<long>)] = DbType.Int64,
            [typeof(Emptyable<ulong>)] = DbType.UInt64,
            [typeof(Emptyable<float>)] = DbType.Single,
            [typeof(Emptyable<double>)] = DbType.Double,
            [typeof(Emptyable<decimal>)] = DbType.Decimal,
            [typeof(Emptyable<bool>)] = DbType.Boolean,
            [typeof(Emptyable<char>)] = DbType.StringFixedLength,
            [typeof(Emptyable<Guid>)] = DbType.Guid,
            [typeof(Emptyable<DateTime>)] = DbType.DateTime,
            [typeof(Emptyable<DateTimeOffset>)] = DbType.DateTimeOffset,
            [typeof(Emptyable<TimeSpan>)] = DbType.Time,
            [typeof(object)] = DbType.Object
        };

        public static DbType GetDbType(Type dataType)
        {
            if(dataType.IsEnum)
            {
                dataType = Enum.GetUnderlyingType(dataType);
            }


            //使用了IRepositoryValue<>接口，获取接口的泛型定义作为数据类型
            //var repositoryValueType = dataType.GetInterface(typeof(IRepositoryValue<>).FullName);
            //if(repositoryValueType !=null)
            //{
            //    dataType = repositoryValueType.GenericTypeArguments[0];
            //}
            DbType dbType = default(DbType);
            if (_typeMap.TryGetValue(dataType, out dbType))
            {
                return dbType;
            }
            throw new InvalidOperationException(string.Format(Strings.NoDbType, dataType.FullName));
        }


        public static int GetMaxLength(this PropertyRepositoryAttribute attr)
        {
            var stringProperty = attr.Property as GeneratedField.StringProperty;
            if(stringProperty != null)
            {
                return stringProperty.MaxLength;
            }
            else
            {
                var sl = attr.Property.GetAttribute<StringLengthAttribute>();
                return sl == null ? 0 : sl.Max;
            }

        }

        public static bool IsASCIIString(this PropertyRepositoryAttribute attr)
        {
            var stringProperty = attr.Property as GeneratedField.StringProperty;
            if (stringProperty != null)
            {
                return stringProperty.IsASCII;
            }
            else
            {
                return attr.Property.GetAttribute<ASCIIStringAttribute>() != null;
            }
        }

        public static bool PropertyIsId(this PropertyRepositoryAttribute attr)
        {
            return attr.PropertyName.EqualsIgnoreCase(EntityObject.IdPropertyName);
        }

        internal static string GetPropertyName(this IDataField field)
        {
            return field.Tip.PropertyName;
        }

        internal static Type GetPropertyType(this IDataField field)
        {
            return field.Tip.PropertyType;
        }

        internal static Type GetReflectedType(this IDataField field)
        {
            return field.Tip.OwnerType;
        }

        //public static bool IsId(this IDataField field)
        //{
        //    return field.Tip.PropertyIsId();
        //}

        /// <summary>
        /// 类型<paramref name="domainObjectType"/>是否为另外一个领域类型的派生类
        /// </summary>
        /// <param name="domainObjectType"></param>
        /// <returns></returns>
        public static bool IsDerived(this Type domainObjectType)
        {
            return DerivedClassAttribute.IsDerived(domainObjectType);
        }

        public static bool IsFrameworkDomainObjectType(this Type domainObjectType)
        {
            return DomainObject.IsFrameworkDomainType(domainObjectType);
        }

        
        public static IEnumerable<DomainProperty> GetProperties(Type objectType)
        {
            if(!objectType.IsDerived()) return DomainProperty.GetProperties(objectType);
            return _getPropertiesByDerived(objectType);
        }

        private static Func<Type, IEnumerable<DomainProperty>> _getPropertiesByDerived = LazyIndexer.Init<Type, IEnumerable<DomainProperty>>((objectType) =>
        {
            var domainProperties = DomainProperty.GetProperties(objectType);
            //对于派生类，我们仅保留派生类和系统框架对象提供的领域属性，自定义基类的属性不保留
            return domainProperties = domainProperties.Where((property) =>
            {
                return property.OwnerType == objectType || property.OwnerType.IsFrameworkDomainObjectType();
            });
        });

        /// <summary>
        /// 当类型为派生类时，仅返回派生类的领域属性，当类型不是派生类时，返回所有属性
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyRepositoryAttribute> GetPropertyTips(Type objectType)
        {
            if(objectType.IsDerived()) return _getPropertyTipsByDerived(objectType);
            return _getPropertyAllTips(objectType);
        }


        /// <summary>
        /// 获取整个继承链包括派生类中定义的属性仓储特性
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyRepositoryAttribute> GetPropertyAllTips(Type objectType)
        {
            return _getPropertyAllTips(objectType);
        }

        /// <summary>
        /// 获得对象定义的所有属性提示
        /// </summary>
        private static Func<Type, IEnumerable<PropertyRepositoryAttribute>> _getPropertyAllTips = LazyIndexer.Init<Type, IEnumerable<PropertyRepositoryAttribute>>((objectType) =>
        {
            var domainProperties = DomainProperty.GetProperties(objectType);
            return domainProperties.Select((p) => p.RepositoryTip).Where((t) => t != null);
        });

        /// <summary>
        /// 获得对象定义的所有属性提示
        /// </summary>
        private static Func<Type, IEnumerable<PropertyRepositoryAttribute>> _getPropertyTipsByDerived = LazyIndexer.Init<Type, IEnumerable<PropertyRepositoryAttribute>>((objectType) =>
        {
            var domainProperties = _getPropertiesByDerived(objectType);
            return domainProperties.Select((p) => p.RepositoryTip).Where((t)=> t!= null);
        });



        public const string SnapshotTime = "SnapshotTime";
        public const string SnapshotLifespan = "SnapshotLifespan";

        ///// <summary>
        ///// 表映射的对象是否为指定类型的对象
        ///// </summary>
        ///// <param name="table"></param>
        ///// <returns></returns>
        //public static bool IsMapObject<T>(this DataTable table)
        //    where T : IDomainObject
        //{
        //    return table.ObjectType != null && table.ObjectType.IsAchieveOrEquals(typeof(T));
        //}



    }
}
