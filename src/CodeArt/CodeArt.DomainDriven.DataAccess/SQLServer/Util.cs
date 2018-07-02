using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using CodeArt.DomainDriven;
using CodeArt.Util;
using CodeArt.Concurrent;


namespace CodeArt.DomainDriven.DataAccess.SQLServer
{
    public static class Util
    {
        public static string GetSqlDbTypeString(DbType dbType)
        {
            return _getSqlDbTypeString(dbType);
        }

        private static Func<DbType, string> _getSqlDbTypeString = LazyIndexer.Init<DbType, string>((dbType) =>
        {
            SqlParameter p = new SqlParameter();
            p.DbType = dbType;
            return p.SqlDbType.ToString().ToLower();
        });


        public static SqlDbType GetSqlDbType(DbType dbType)
        {
            return _getSqlDbType(dbType);
        }


        private static Func<DbType, SqlDbType> _getSqlDbType = LazyIndexer.Init<DbType, SqlDbType>((dbType) =>
        {
            SqlParameter p = new SqlParameter();
            p.DbType = dbType;
            return p.SqlDbType;
        });


        public static string GetSqlDbTypeString(Type type)
        {
            return _getSqlDbTypeStringByNetType(type);
        }

        private static Func<Type, string> _getSqlDbTypeStringByNetType = LazyIndexer.Init<Type, string>((type) =>
        {
            var sqlDbType = GetSqlDbType(type);
            return sqlDbType.ToString().ToLower();
        });

        private static SqlDbType GetSqlDbType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                case TypeCode.Byte:
                    return SqlDbType.TinyInt;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;
                case TypeCode.Double:
                    return SqlDbType.Float;
                case TypeCode.Int16:
                    return SqlDbType.SmallInt;
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;
                case TypeCode.SByte:
                    return SqlDbType.TinyInt;
                case TypeCode.Single:
                    return SqlDbType.Real;
                case TypeCode.String:
                    return SqlDbType.NVarChar;
                case TypeCode.UInt16:
                    return SqlDbType.SmallInt;
                case TypeCode.UInt32:
                    return SqlDbType.Int;
                case TypeCode.UInt64:
                    return SqlDbType.BigInt;
                case TypeCode.Object:
                    {
                        if (type == typeof(Guid))
                            return SqlDbType.UniqueIdentifier;
                        break;
                    }
            }
            throw new DataAccessException(string.Format(Strings.NotSupportDbType, type.Name));
        }

    }
}
