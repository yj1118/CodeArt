using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CodeArt.DomainDriven.DataAccess
{
    public abstract class DbField
    {
        public string Name
        {
            get;
            private set;
        }

        public abstract Type ValueType
        {
            get;
        }

        public DbField(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 根据类型创造数据库字段，字符串类型请直接构造，不要调用该方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DbField Create<T>(string name)
        {
            return Create(name, typeof(T));
        }

        /// <summary>
        /// 根据类型创造数据库字段，字符串类型请直接构造，不要调用该方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbField Create(string name, Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return new BooleanField(name);
                case TypeCode.Byte:
                    return new ByteField(name);
                case TypeCode.DateTime:
                    return new DataTimeField(name);
                case TypeCode.Decimal:
                    return new DecimalField(name);
                case TypeCode.Double:
                    return new DoubleField(name);
                case TypeCode.Int16:
                    return new ShortField(name);
                case TypeCode.Int32:
                    return new IntField(name);
                case TypeCode.Int64:
                    return new LongField(name);
                case TypeCode.SByte:
                    return new ByteField(name);
                case TypeCode.Single:
                    return new FloatField(name);
                case TypeCode.UInt16:
                    return new ShortField(name);
                case TypeCode.UInt32:
                    return new IntField(name);
                case TypeCode.UInt64:
                    return new LongField(name);
                case TypeCode.Object:
                    {
                        if (type == typeof(Guid))
                            return new GuidField(name);
                        break;
                    }
            }
            throw new DataAccessException(string.Format(Strings.NotFoundDbField, type.Name));
        }

    }
}
